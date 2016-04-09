using System;

namespace ClashRoyaleProxy
{
    class Keys
    {
        /// <summary>
        /// The generated private key, according to the modded public key.
        /// </summary>
        public static byte[] GeneratedPrivateKey
        {
            get
            {
                return Helper.HexToByteArray("1891d401fadb51d25d3a9174d472a9f691a45b974285d47729c45c6538070d85");
            }
        }

        /// <summary>
        /// The modded Clash Royale public key.
        /// Offset 0x0039A01C [ARM / ANDROID]
        /// </summary>
        public static byte[] ModdedPublicKey
        {
            get
            {
                return Helper.HexToByteArray("72f1a4a4c48e44da0c42310f800e96624e6dc6a641a9d41c3b5039d8dfadc27e");
            }
        }

        /// <summary>
        /// The original, unmodified Clash Royale public key.
        /// Offset 0x0039A01C [ARM / ANDROID]
        /// </summary>
        public static byte[] OriginalPublicKey
        {
            get
            {
                return Helper.HexToByteArray("ba105f0d3a099414d154046f41d80cf122b49902eab03b78a912f3c66dba2c39");

            }
        }

        /// <summary>
        /// An old nonce used by Supercell.
        /// </summary>
        public static byte[] Nonce
        {
            get
            {
                return System.Text.Encoding.UTF8.GetBytes("nonce");
            }
        }
    }
}
