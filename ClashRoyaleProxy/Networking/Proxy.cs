using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ClashRoyaleProxy
{
    class Proxy
    {
        private static List<Client> ClientPool = new List<Client>();
        private const int PORT = 9339;
        private const int MAX_CONNECTIONS = 100;

        /// <summary>
        /// Starts the proxy
        /// </summary>
        public static void Start()
        {
            // Bind a new socket to the local EP
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

        /// <summary>
        /// Stops the proxy
        /// </summary>
        public static void Stop()
        {
            for (int i = 0; i < ClientPool.Count; i++)
            {
                ClientPool[i].Dequeue();
            }
            ClientPool.Clear();
        }
    }
}
