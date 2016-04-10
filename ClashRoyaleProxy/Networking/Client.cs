using System;
using System.Net;
using System.Net.Sockets;

namespace ClashRoyaleProxy
{
    class Client
    {
        private const string CRHost_IOS = "game.clashroyaleapp.com";
        private const string CRHost_ANDROID = "gamec.clashroyaleapp.com";
        private const int CRPort = 9339;

        public Socket ClientSocket, ServerSocket;

        public Client(Socket s)
        {
            ClientSocket = s;
        }

        /// <summary>
        /// Enqueues the client
        /// </summary>
        public void Enqueue()
        {
            // Connect to the official supercell server
            IPHostEntry ipHostInfo = Dns.GetHostEntry(CRHost_ANDROID);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, CRPort);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Connect(remoteEndPoint);

            // Start async recv/send procedure
            Logger.Log("Proxy attached to " + CRHost_ANDROID + " (" + ServerRemoteAdr + ")!", LogType.INFO);
            Logger.Log("Starting async recv/send threads..", LogType.INFO);
            new ReceiveSendThread(ClientSocket, ServerSocket);              
        }

        /// <summary>
        /// Dequeues the client
        /// </summary>
        public void Dequeue()
        {
            ClientSocket.Disconnect(false);
            ServerSocket.Disconnect(false);
        }
  
        /// <summary>
        /// Client IP-address
        /// </summary>
        public IPAddress ClientRemoteAdr
        {
            get
            {
                return ((IPEndPoint)ClientSocket.RemoteEndPoint).Address;
            }
        }

        /// <summary>
        /// Clash Royale Server IP-address
        /// </summary>
        public IPAddress ServerRemoteAdr
        {
            get
            {
                return ((IPEndPoint)ServerSocket.RemoteEndPoint).Address;
            }
        }
    }
}
