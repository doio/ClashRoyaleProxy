using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public void Enqueue()
        {
            // Connect to the official supercell server
            IPHostEntry ipHostInfo = Dns.GetHostEntry(CRHost_ANDROID);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, CRPort);
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Connect(remoteEndPoint);
            Logger.Log("Proxy attached to " + CRHost_ANDROID + " (" + ServerRemoteAdr + ")!", LogType.INFO);
            Logger.Log("Starting async recv/send threads..", LogType.INFO);
            new ReceiveSendThread(ClientSocket, ServerSocket);              
        }
   
        public Socket Socket_Client
        {
            get
            {
                return ClientSocket;
            }
        }

        public Socket Socket_Server
        {
            get
            {
                return ServerSocket;
            }
        }
        public IPAddress ClientRemoteAdr
        {
            get
            {
                return ((IPEndPoint)ClientSocket.RemoteEndPoint).Address;
            }
        }

        public IPAddress ServerRemoteAdr
        {
            get
            {
                return ((IPEndPoint)ServerSocket.RemoteEndPoint).Address;
            }
        }
    }
}
