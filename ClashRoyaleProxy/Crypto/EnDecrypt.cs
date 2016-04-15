using System;
using System.Linq;
using System.Text;
using Blake2Sharp;

namespace ClashRoyaleProxy
{
    class EnDecrypt
    {
        // Constants
        private const int KeyLength = 32, NonceLength = 24, SessionLength = 24;

        // A custom keypair used for en/decryption 
        private static KeyPair CustomKeyPair = new KeyPair();

        // The 32-byte prefixed public key from cipher 10101
        private static byte[] _10101_PublicKey = new byte[KeyLength];

        // The 24-byte prefixed session key from plain 10101
        private static byte[] _10101_SessionKey = new byte[SessionLength];

        // The 24-byte prefixed nonce from plain 10101
        private static byte[] _10101_Nonce = new byte[NonceLength];

        // The 24-byte prefixed nonce from plain 20103/20104
        private static byte[] _20103_20104_Nonce = new byte[NonceLength];

        // The 32-byte prefixed shared key from plain 20103/20104
        private static byte[] _20103_20104_SharedKey = new byte[KeyLength];

        // Default Blake2b
        private static Hasher Blake2b = Blake2B.Create(new Blake2BConfig() { OutputSizeInBytes = 24 });

        /// <summary>
        /// Encrypts a client/server packet 
        /// </summary>
        /// <param name="p">Packet to encrypt</param>
        /// <returns>Encrypted payload</returns>
        public static byte[] EncryptPacket(Packet p)
        {
            int packetID = p.ID;
            byte[] decryptedPayload = p.DecryptedPayload;
            byte[] encryptedPayload;

            if (packetID == 10100 || packetID == 20100)
            {
                // Both Session (10100) and SessionOk (20100) packets are not encrypted
                // Thus.. just return the decrypted payload
                return decryptedPayload;
            }
            else if (packetID == 10101)
            {
                // The encrypted Login (10101) requires a nonce being calculated by the custom PK & the original PK
                Blake2b.Init();
                Blake2b.Update(CustomKeyPair.PublicKey);
                Blake2b.Update(Keys.OriginalPublicKey);
                var tmpNonce = Blake2b.Finish();

                // The decrypted payload has to be prefixed with the nonce from plain 10101
                decryptedPayload = _10101_SessionKey.Concat(_10101_Nonce).Concat(decryptedPayload).ToArray();
                // Encrypt the payload with the custom NaCl 
                encryptedPayload = CustomNaCl.CreatePublicBox(decryptedPayload, tmpNonce, CustomKeyPair.SecretKey, Keys.OriginalPublicKey);
                // The encrypted payload has to be prefixed with the custom PK
                encryptedPayload = CustomKeyPair.PublicKey.Concat(encryptedPayload).ToArray();
            }
            else if (packetID == 20103 || packetID == 20104)
            {
                // The encrypted LoginFailed / LoginOk (20103/20104) requires a nonce being calculated by the nonce from 10101, the PK from 10101 and the client PK            
                Blake2b.Init();
                Blake2b.Update(_10101_Nonce);
                Blake2b.Update(_10101_PublicKey);
                Blake2b.Update(Keys.ModdedPublicKey);
                var tmpNonce = Blake2b.Finish();

                // The decrypted payload has to be prefixed with the nonce from 20103/20104 and the sharedkey from 20103/20104
                decryptedPayload = _20103_20104_Nonce.Concat(_20103_20104_SharedKey).Concat(decryptedPayload).ToArray();
                // Encrypt the payload with the custom NaCl
                encryptedPayload = CustomNaCl.CreatePublicBox(decryptedPayload, tmpNonce, Keys.GeneratedPrivateKey, _10101_PublicKey);
            }
            else
            {
                // We're dealing with another packet. Depends whether it's a client packet or not.
                if (p.Destination == DataDestination.DATA_FROM_CLIENT)
                {
                    encryptedPayload = CustomNaCl.CreateSecretBox(decryptedPayload, _10101_Nonce, _20103_20104_SharedKey).Skip(16).ToArray();
                }
                else
                {
                    encryptedPayload = CustomNaCl.CreateSecretBox(decryptedPayload, _20103_20104_Nonce, _20103_20104_SharedKey).Skip(16).ToArray();
                }
            }
            return encryptedPayload;
        }

        /// <summary>
        /// Decrypts a packet
        /// </summary>
        public static byte[] DecryptPacket(Packet p)
        {
            int packetID = p.ID;
            byte[] encryptedPayload = p.Payload;
            byte[] decryptedPayload;

            if (packetID == 10100 || packetID == 20100)
            {
                // Both Session (10100) and SessionOk (20100) packets are not encrypted
                // Thus.. just return the encrypted payload
                return encryptedPayload;
            }
            else if (packetID == 10101)
            {
                // The decrypted Login (10101) requires a nonce being calculated by the PK & the modded PK
                _10101_PublicKey = encryptedPayload.Take(32).ToArray();
                Blake2b.Init();
                Blake2b.Update(_10101_PublicKey);
                Blake2b.Update(Keys.ModdedPublicKey);
                var tmpNonce = Blake2b.Finish();

                // Decrypt the payload the custom NaCl
                decryptedPayload = CustomNaCl.OpenPublicBox(encryptedPayload.Skip(32).ToArray(), tmpNonce, Keys.GeneratedPrivateKey, _10101_PublicKey);
                _10101_SessionKey = decryptedPayload.Take(24).ToArray();
                _10101_Nonce = decryptedPayload.Skip(24).Take(24).ToArray();
                decryptedPayload = decryptedPayload.Skip(48).ToArray();
            }
            else if (packetID == 20103 || packetID == 20104)
            {
                // The decrypted LoginFailed / LoginOk (20103/20104) requires a nonce being calculated by the nonce from 10101, the custom PK & the original PK                   
                Blake2b.Init();
                Blake2b.Update(_10101_Nonce);
                Blake2b.Update(CustomKeyPair.PublicKey);
                Blake2b.Update(Keys.OriginalPublicKey);
                var tmpNonce = Blake2b.Finish();

                // Decrypt the payload with the custom NaCl
                decryptedPayload = CustomNaCl.OpenPublicBox(encryptedPayload, tmpNonce, CustomKeyPair.SecretKey, Keys.OriginalPublicKey);
                _20103_20104_Nonce = decryptedPayload.Take(24).ToArray();
                _20103_20104_SharedKey = decryptedPayload.Skip(24).Take(32).ToArray();
                decryptedPayload = decryptedPayload.Skip(56).ToArray();
            }
            else
            {
                if (p.Destination == DataDestination.DATA_FROM_CLIENT)
                {
                    _10101_Nonce.Increment();
                    decryptedPayload = CustomNaCl.OpenSecretBox(new byte[16].Concat(encryptedPayload).ToArray(), _10101_Nonce, _20103_20104_SharedKey);
                }
                else
                {
                    _20103_20104_Nonce.Increment();
                    decryptedPayload = CustomNaCl.OpenSecretBox(new byte[16].Concat(encryptedPayload).ToArray(), _20103_20104_Nonce, _20103_20104_SharedKey);
                }
            }
            return decryptedPayload;
        }
    }
}
