using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClashRoyaleProxy
{
    class Proxy
    {
        static List<Client> ClientPool = new List<Client>();
        const int PORT = 9339;
        const int MAX_CONNECTIONS = 100;

        /// <summary>
        /// Starts the actual proxy
        /// </summary>
        public static void Start()
        {
            // Bind a new socket to 127.0.0.1 aka. localhost
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            Socket clientListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientListener.Bind(endPoint);
            clientListener.Listen(MAX_CONNECTIONS);
            Logger.Log("Successfully bound socket to " + endPoint.Address + "!", LogType.INFO);
            Logger.Log("Listening for incoming connections..", LogType.INFO);
            
            while (true)
            {
                Socket clientSocket = clientListener.Accept();
                // Client connected, let's enqueue him!
                Client client = new Client(clientSocket);
                Logger.Log("Remote connection from client #" + (ClientPool.ToArray().Length + 1) + " (" + client.ClientRemoteAdr + "), enqueuing..", LogType.INFO);
                ClientPool.Add(client);
                client.Enqueue();
            }
        }

    }
}
