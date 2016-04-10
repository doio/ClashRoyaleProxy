using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ClashRoyaleProxy
{
    static class Extensions
    {
        /// <summary>
        /// Returns if a socket disconnected
        /// </summary>
        public static bool Disconnected(this Socket socket)
        {
            try
            {
                return (socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return true;
            }
        }

        /// <summary>
        /// Returns if an object disposed by using the C++ sizeof()-like method
        /// </summary>
        public static bool Disposed(this object s)
        {
            return Marshal.SizeOf(s) > 0;
        }

        // Add datatypes to a generic list of bytes
        public static void AddShort(this List<byte> list, short data)
        {
            list.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void AddInt(this List<byte> list, int data)
        {
            list.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void AddLong(this List<byte> list, long data)
        {
            list.AddRange(BitConverter.GetBytes(data).Reverse());
        }

        public static void AddString(this List<byte> list, string data)
        {
            if (data == null)
                list.AddRange(BitConverter.GetBytes(-1).Reverse());
            else
            {
                list.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetByteCount(data)).Reverse());
                list.AddRange(Encoding.UTF8.GetBytes(data));
            }
        }

        // Read datatypes from a byte array
        public static short ReadShortWithEndian(this BinaryReader br)
        {
            var a16 = br.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(a16);
            return BitConverter.ToInt16(a16, 0);
        }
        public static int ReadIntWithEndian(this BinaryReader br)
        {
            var a32 = br.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(a32);
            return BitConverter.ToInt32(a32, 0);
        }

        public static long ReadLongWithEndian(this BinaryReader br)
        {
            var a64 = br.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(a64);
            return BitConverter.ToInt64(a64, 0);
        }

        public static string ReadString(this BinaryReader br)
        {
            int lengthOfUTF8Str = br.ReadIntWithEndian();
            string UTF8Str;

            if (lengthOfUTF8Str > -1)
            {
                if (lengthOfUTF8Str > 0)
                {
                    var tmp = br.ReadBytes(lengthOfUTF8Str);
                    UTF8Str = Encoding.UTF8.GetString(tmp);
                }
                else
                {
                    UTF8Str = string.Empty;
                }
            }
            else
                UTF8Str = null;
            return UTF8Str;
        }

        public static int ReadMedium(this BinaryReader br)
        {
            var tmp = br.ReadBytes(3);
            return (0x00 << 24) | (tmp[0] << 16) | (tmp[1] << 8) | tmp[2];
        }

        public static ushort ReadUShortWithEndian(this BinaryReader br)
        {
            var a16 = br.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(a16);
            return BitConverter.ToUInt16(a16, 0);
        }

        public static uint ReadUIntWithEndian(this BinaryReader br)
        {
            var a32 = br.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(a32);
            return BitConverter.ToUInt32(a32, 0);
        }

        public static ulong ReadULongWithEndian(this BinaryReader br)
        {
            var a64 = br.ReadBytes(8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(a64);
            return BitConverter.ToUInt64(a64, 0);
        }
    }
}
