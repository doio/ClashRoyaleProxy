using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ClashRoyaleProxy
{
    // Helper class for my customized TweetNaCl version
    class CustomNaCl
    {
        /// <summary>
        /// Increments the specified nonce.
        /// </summary>
        public static void NonceIncr(byte[] nonce)
        {
            /*
            void sodium_increment(unsigned char *n, const size_t nlen) 
            { 
              size_t        i = 0U; 
              uint_fast16_t c = 1U; 
 
              for (; i < nlen; i++) { 
              c += (uint_fast16_t) n[i]; 
              n[i] = (unsigned char) c; 
              c >>= 8; 
            }
            */
            for (int j = 0; j < 2; j++)
            {
                ushort c = 1;
                for (UInt32 i = 0; i < nonce.Length; i++)
                {
                    c += (ushort)nonce[i];
                    nonce[i] = (byte)c;
                    c >>= 8;
                }
            }
        }
    }
}
