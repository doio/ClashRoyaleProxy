using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blake2Sharp;

namespace ClashRoyaleProxy
{
    class EnDecrypt
    {
        private static KeyPair ClientKeyPair = new KeyPair();
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
                    var blake2b = Blake2B.Create(new Blake2BConfig() { OutputSizeInBytes = 24 });
                    blake2b.Init();
                    blake2b.Update(ClientKeyPair.PublicKey);
                    blake2b.Update(Keys.OriginalPublicKey);
                    byte[] nonce = blake2b.Finish();

                    decrypted = SessionKeyLogin.Concat(NonceLogin).Concat(decrypted).ToArray();
                    encrypted = CustomNaCl.CreatePublicBox(decrypted, nonce, ClientKeyPair.SecretKey, Keys.OriginalPublicKey);
                    encrypted = ClientKeyPair.PublicKey.Concat(encrypted).ToArray();
                }
                else if (p.ID == 20103 || p.ID == 20104)
                {
                    // LoginOk / LoginFailed
                    var blake2b = Blake2B.Create(new Blake2BConfig() { OutputSizeInBytes = 24 });
                    blake2b.Init();
                    blake2b.Update(NonceLogin);
                    blake2b.Update(PKLogin);
                    blake2b.Update(Keys.ModdedPublicKey);
                    byte[] nonce = blake2b.Finish();

                    decrypted = NonceLoginOkFailed.Concat(SharedKeyLoginOkFailed).Concat(decrypted).ToArray();
                    encrypted = CustomNaCl.CreatePublicBox(decrypted, nonce, Keys.GeneratedPrivateKey, PKLogin);
                }
                else if (p.ID == 24101)
                {
                    // MODDING
                    int Pos = 0;
                    foreach (byte b in decrypted)
                    {
                        if (b == 0x1c && decrypted[Pos + 1] == 0x01)
                        {
                            // Arrows
                            decrypted[Pos + 2] = 0x0B;
                        }
                        if (b == 0x1a && decrypted[Pos + 1] == 0x00)
                        {
                            // Knight
                            decrypted[Pos + 2] = 0x0B;
                        }
                        if (b == 0x1a && decrypted[Pos + 1] == 0x01)
                        {
                            // Archers
                            decrypted[Pos + 2] = 0x0B;
                        }
                        if (b == 0x1a && decrypted[Pos + 1] == 0x0d)
                        {
                            // Bomber
                            decrypted[Pos + 2] = 0x0B;
                        }
                        if (b == 0x1c && decrypted[Pos + 1] == 0x00)
                        {
                            // Fireball
                            decrypted[Pos + 2] = 0x09;
                        }
                        if (b == 0x1a && decrypted[Pos + 1] == 0x03)
                        {
                            // Giant
                            decrypted[Pos + 2] = 0x09;
                        }
                        Pos++;
                    }
                    encrypted = CustomNaCl.CreateSecretBox(decrypted, NonceLoginOkFailed, SharedKeyLoginOkFailed).Skip(16).ToArray();
                }
                else
                {
                    if (p.Destination == DataDestination.DATA_FROM_SERVER)
                    {
                        encrypted = CustomNaCl.CreateSecretBox(decrypted, NonceLoginOkFailed, SharedKeyLoginOkFailed).Skip(16).ToArray();
                    }
                    else
                    {
                        encrypted = CustomNaCl.CreateSecretBox(decrypted, NonceLogin, SharedKeyLoginOkFailed).Skip(16).ToArray();
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
                    var blake2b = Blake2B.Create(new Blake2BConfig() { OutputSizeInBytes = 24 });
                    blake2b.Init();
                    blake2b.Update(PKLogin);
                    blake2b.Update(Keys.ModdedPublicKey);
                    byte[] nonce = blake2b.Finish();

                    encrypted = encrypted.Skip(32).ToArray();
                    decrypted = CustomNaCl.OpenPublicBox(encrypted, nonce, Keys.GeneratedPrivateKey, PKLogin);
                    SessionKeyLogin = decrypted.Take(24).ToArray();
                    NonceLogin = decrypted.Skip(24).Take(24).ToArray();
                    decrypted = decrypted.Skip(48).ToArray();
                }
                else if (p.ID == 20103 || p.ID == 20104)
                {
                    // LoginFailed / LoginOk
                    var blake2b = Blake2B.Create(new Blake2BConfig() { OutputSizeInBytes = 24 });
                    blake2b.Init();
                    blake2b.Update(NonceLogin);
                    blake2b.Update(ClientKeyPair.PublicKey);
                    blake2b.Update(Keys.OriginalPublicKey);
                    byte[] nonce = blake2b.Finish();

                    decrypted = CustomNaCl.OpenPublicBox(encrypted, nonce, ClientKeyPair.SecretKey, Keys.OriginalPublicKey);
                    NonceLoginOkFailed = decrypted.Take(24).ToArray();
                    SharedKeyLoginOkFailed = decrypted.Skip(24).Take(32).ToArray();
                    decrypted = decrypted.Skip(56).ToArray();
                }
                else
                {
                    if (p.Destination == DataDestination.DATA_FROM_SERVER)
                    {
                        CustomNaCl.NonceIncr(NonceLoginOkFailed);
                        decrypted = CustomNaCl.OpenSecretBox(new byte[16].Concat(encrypted).ToArray(), NonceLoginOkFailed, SharedKeyLoginOkFailed);
                    }
                    else
                    {
                        CustomNaCl.NonceIncr(NonceLogin);
                        decrypted = CustomNaCl.OpenSecretBox(new byte[16].Concat(encrypted).ToArray(), NonceLogin, SharedKeyLoginOkFailed);
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
