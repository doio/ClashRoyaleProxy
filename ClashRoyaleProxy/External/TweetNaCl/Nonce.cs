using System;

namespace ClashRoyaleProxy
{
    class Nonce
    {
        private const int NONCELENGTH = 24;
        private byte[] nonce = new byte[NONCELENGTH];

        /// <summary>
        /// Nonce constructor
        /// </summary>
        /// <param name="n">The 24 byte nonce</param>
        public Nonce(byte[] n)
        {
            this.nonce = n;
        }

        /// <summary>
        /// Increments the nonce
        /// </summary>
        public void Incremenet(int timesToIncrease = 2)
        {
            /*
            int __cdecl sodium_increment(unsigned char nonce, unsigned int nonceLength)
            {
                __int64 v2 = 1i64; // rax@1
                do
                {
                     LODWORD(v2) = *(_BYTE *)(HIDWORD(v2) + a1) + (_DWORD)v2;
                     *(_BYTE *)(HIDWORD(v2)++ + a1) = v2;
                     LODWORD(v2) = (unsigned int)v2 >> 8;
                }
                while ( HIDWORD(v2) < a2 );
             return v2;
            }
            */
            for (int j = 0; j < timesToIncrease; j++)
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

        /// <summary>
        /// Decrements the nonce
        /// </summary>
        public void Decrement(int timesToDecrease = 2)
        {
            for (int j = 0; j < timesToDecrease; j++)
            {
                ushort c = 1;
                for (UInt32 i = 0; i < nonce.Length; i++)
                {
                    c -= (ushort)nonce[i];
                    nonce[i] = (byte)c;
                    c <<= 8;
                }
            }
        }
    }
}
