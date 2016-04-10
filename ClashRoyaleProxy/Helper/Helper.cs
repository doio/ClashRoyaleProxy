using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyaleProxy
{
    class Helper
    {
        /// <summary>
        /// Uses LINQ to convert a hexlified string to a byte array.
        /// </summary>
        public static byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Converts a byte array to a hexlified string.
        /// </summary>
        public static string ByteArrayToHex(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", " ").ToUpper();
        }

        /// <summary>
        /// Returns opened instances
        /// </summary>
        public static int OpenedInstances
        {
            get
            {
                return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length;
            }
        }

        /// <summary>
        /// Returns Proxy-Version in the following format:
        /// v1.2.3
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                return "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Remove(5);
            }
        }
    }
}
