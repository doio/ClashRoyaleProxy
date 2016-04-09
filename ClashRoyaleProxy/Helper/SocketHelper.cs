using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyaleProxy
{
    static class SocketHelper
    {
        public static bool Disconnected(this Socket socket)
        {
            try
            {
                return (socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return true; }
        }

        static bool Disposed(this Socket s)
        {
            return Marshal.SizeOf(s) > 0;
        }
    }
}
