using System.Net.Sockets;

namespace ClashRoyaleProxy
{
    public class State
    {
        public Socket socket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public byte[] packet = new byte[0];

    }
}
