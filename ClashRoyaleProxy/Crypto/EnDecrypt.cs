using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodium;
using Blake2Sharp;

namespace ClashRoyaleProxy
{
    class EnDecrypt
    {
        private static KeyPair ClientKeyPair = PublicKeyBox.GenerateKeyPair();
        private static byte[] PKLogin { get; set; }
        private static byte[] SessionKeyLogin { get; set; }
        private static byte[] NonceLogin { get; set; }
        private static byte[] NonceLoginOkFailed { get; set; }
        private static byte[] SharedKeyLoginOkFailed { get; set; }

        /// <summary>
        /// Encrypts a packet
        /// </summary>
        public static byte[] EncryptPacket(Packet p)
        {
            try
            {
                byte[] decrypted = p.DecryptedPayload;
                byte[] encrypted = null;

                if (p.ID == 10100 || p.ID == 20100)
                {
                    // SessionPacket
                    return decrypted;
                }
                else if (p.ID == 10101)
                {
                    // LoginPacket
                    byte[] nonce = GenericHash.Hash(ClientKeyPair.PublicKey.Concat(Keys.OriginalPublicKey).ToArray(), null, 24);
                    decrypted = SessionKeyLogin.Concat(NonceLogin).Concat(decrypted).ToArray();
                    encrypted = PublicKeyBox.Create(decrypted, nonce, ClientKeyPair.PrivateKey, Keys.OriginalPublicKey);
                    encrypted = ClientKeyPair.PublicKey.Concat(encrypted).ToArray();
                }
                else if (p.ID == 20103 || p.ID == 20104)
                {
                    // LoginOk / LoginFailed
                    byte[] nonce = GenericHash.Hash(NonceLogin.Concat(PKLogin).Concat(Keys.ModdedPublicKey).ToArray(), null, 24);
                    decrypted = NonceLoginOkFailed.Concat(SharedKeyLoginOkFailed).Concat(decrypted).ToArray();
                    encrypted = PublicKeyBox.Create(decrypted, nonce, Keys.GeneratedPrivateKey, PKLogin);
                }
                else
                {
                    if (p.Destination == DataDestination.DATA_FROM_SERVER)
                    {
                        encrypted = SecretBox.Create(decrypted, NonceLoginOkFailed, SharedKeyLoginOkFailed).Skip(16).ToArray();
                    }
                    else
                    {
                        encrypted = SecretBox.Create(decrypted, NonceLogin, SharedKeyLoginOkFailed).Skip(16).ToArray();
                    }
                }
                return encrypted;
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to encrypt packet " + p.ID + " (" + ex.GetType() + ")..", LogType.EXCEPTION);
            }
            return null;
        }

        /// <summary>
        /// Decrypts a packet
        /// </summary>
        public static byte[] DecryptPacket(Packet p)
        {
            try
            {
                byte[] encrypted = p.Payload;
                byte[] decrypted = null;

                if (p.ID == 10100 || p.ID == 20100)
                {
                    // Both packets 10100 and 20100 are not encrypted
                    return encrypted;
                }
                else if (p.ID == 10101)
                {
                    // LoginPacket
                    PKLogin = encrypted.Take(32).ToArray();
                    byte[] nonce = GenericHash.Hash(PKLogin.Concat(Keys.ModdedPublicKey).ToArray(), null, 24);
                    encrypted = encrypted.Skip(32).ToArray();
                    decrypted = PublicKeyBox.Open(encrypted, nonce, Keys.GeneratedPrivateKey, PKLogin);
                    SessionKeyLogin = decrypted.Take(24).ToArray();
                    NonceLogin = decrypted.Skip(24).Take(24).ToArray();
                    decrypted = decrypted.Skip(48).ToArray();
                }
                else if (p.ID == 20103 || p.ID == 20104)
                {
                    byte[] nonce = GenericHash.Hash(NonceLogin.Concat(ClientKeyPair.PublicKey).Concat(Keys.OriginalPublicKey).ToArray(), null, 24);
                    decrypted = PublicKeyBox.Open(encrypted, nonce, ClientKeyPair.PrivateKey, Keys.OriginalPublicKey);
                    NonceLoginOkFailed = decrypted.Take(24).ToArray();
                    SharedKeyLoginOkFailed = decrypted.Skip(24).Take(32).ToArray();
                    decrypted = decrypted.Skip(56).ToArray();
                }
                else
                {
                    if (p.Destination == DataDestination.DATA_FROM_SERVER)
                    {
                        NonceLoginOkFailed = Utilities.Increment(Utilities.Increment(NonceLoginOkFailed));
                        decrypted = SecretBox.Open(new byte[16].Concat(encrypted).ToArray(), NonceLoginOkFailed, SharedKeyLoginOkFailed);
                    }
                    else
                    {
                        NonceLogin = Utilities.Increment(Utilities.Increment(NonceLogin));
                        decrypted = SecretBox.Open(new byte[16].Concat(encrypted).ToArray(), NonceLogin, SharedKeyLoginOkFailed);
                    }
                }
                return decrypted;
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to decrypt packet " + p.ID + " (" + ex.GetType() + ")..", LogType.EXCEPTION);
            }
            return null;
        
    }
    }
}
