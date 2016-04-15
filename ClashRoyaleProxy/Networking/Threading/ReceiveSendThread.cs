using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ClashRoyaleProxy
{
    class ReceiveSendThread : IDisposable
    {
        public Socket ClientSocket, ServerSocket;

        /// <summary>
        /// Async send/receive thread
        /// </summary>
        public ReceiveSendThread(Socket cs, Socket ss)
        {
            this.ClientSocket = cs;
            this.ServerSocket = ss;
            Task.Factory.StartNew(new Action(() =>
            {
                ClientState clientState = new ClientState() { socket = ClientSocket };
                ServerState serverState = new ServerState() { socket = ServerSocket };

                ServerSocket.BeginReceive(serverState.buffer, 0, State.BufferSize, 0, new AsyncCallback(DataReceived), serverState);
                ClientSocket.BeginReceive(clientState.buffer, 0, State.BufferSize, 0, new AsyncCallback(DataReceived), clientState);
            }));
        }

        /// <summary>
        /// DataReceive callback
        /// </summary>
        private void DataReceived(IAsyncResult ar)
        {
            State state = (State)ar.AsyncState;
            Socket socket = state.socket;
            int bytesReceived = socket.EndReceive(ar);
            Handle(bytesReceived, socket, state);
            socket.BeginReceive(state.buffer, 0, State.BufferSize, 0, new AsyncCallback(DataReceived), state);
        }

        /// <summary>
        /// DataSent callback
        /// </summary>
        private void DataSent(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            int sent = socket.EndSend(ar);
        }

        /// <summary>
        /// Handles the received data
        /// </summary>
        private void Handle(int bytesReceived, Socket socket, State state)
        {
            int bytesRead = 0;
            int payloadLength, bytesAvailable, bytesNeeded;
            while (bytesRead < bytesReceived)
            {
                bytesAvailable = bytesReceived - bytesRead;
                if (bytesReceived > 0)
                {
                    if (state.packet.Length >= 7)
                    {
                        payloadLength = BitConverter.ToInt32(new byte[1].Concat(state.packet.Skip(2).Take(3)).Reverse().ToArray(), 0);
                        bytesNeeded = payloadLength - (state.packet.Length - 7);
                        if (bytesAvailable >= bytesNeeded)
                        {
                            state.packet = state.packet.Concat(state.buffer.Skip(bytesRead).Take(bytesNeeded)).ToArray();
                            bytesRead += bytesNeeded;
                            bytesAvailable -= bytesNeeded;
                            if (state.GetType() == typeof(ClientState))
                            {
                                Packet clientPacket = new Packet(state.packet, DataDestination.DATA_FROM_CLIENT);
                                Logger.LogPacket(clientPacket);
                                ServerSocket.Send(clientPacket.Raw);
                            }
                            else if (state.GetType() == typeof(ServerState))
                            {
                                Packet serverPacket = new Packet(state.packet, DataDestination.DATA_FROM_SERVER);
                                Logger.LogPacket(serverPacket);
                                ClientSocket.Send(serverPacket.Raw);
                            }
                            state.packet = new byte[0];
                        }
                        else
                        {
                            state.packet = state.packet.Concat(state.buffer.Skip(bytesRead).Take(bytesAvailable)).ToArray();
                            bytesRead = bytesReceived;
                            bytesAvailable = 0;
                        }
                    }
                    else if (bytesAvailable >= 7)
                    {
                        state.packet = state.packet.Concat(state.buffer.Skip(bytesRead).Take(7)).ToArray();
                        bytesRead += 7;
                        bytesAvailable -= 7;
                    }
                    else
                    {
                        state.packet = state.packet.Concat(state.buffer.Skip(bytesRead).Take(bytesAvailable)).ToArray();
                        bytesRead = bytesReceived;
                        bytesAvailable = 0;
                    }

                }
            }

        }
        public virtual void Dispose()
        {
            ClientSocket.Disconnect(false);
            ServerSocket.Disconnect(false);
            GC.SuppressFinalize(this);
        }
    }
}