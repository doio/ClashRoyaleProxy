using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace ClashRoyaleProxy
{
    class Packet
    {
        private byte[] rawPacket;
        private int packetID;
        private int payloadLen;
        private int messageVer;
        private byte[] payload;
        private string packetType;
        private byte[] encryptedPayload;
        private byte[] decryptedPayload;
        private DataDestination destination;

        public Packet(byte[] buf, DataDestination d)
        {
            // Read content
            this.rawPacket = buf;
            this.destination = d;
            this.packetID = BitConverter.ToInt32(new byte[2].Concat(buf.Take(2)).Reverse().ToArray(), 0);
            this.payloadLen = BitConverter.ToInt32(new byte[1].Concat(buf.Skip(2).Take(3)).Reverse().ToArray(), 0);
            this.messageVer = BitConverter.ToInt32(new byte[2].Concat(buf.Skip(2).Skip(3).Take(2)).Reverse().ToArray(), 0);
            this.payload = buf.Skip(7).ToArray();
            this.packetType = PacketType.GetPacketTypeByID(packetID);

            // En/Decrypt payload
            this.decryptedPayload = EnDecrypt.DecryptPacket(this);
            this.encryptedPayload = EnDecrypt.EncryptPacket(this);
        }

        /// <summary>
        /// Raw, encrypted packet (header included)
        /// 7 byte header + n byte payload
        /// Reverse() because of little endian byte order
        /// </summary>
        public byte[] Raw
        {
            get
            {
                return BitConverter.GetBytes(ID).Reverse().Skip(2).Concat(BitConverter.GetBytes(EncryptedPayload.Length).Reverse().Skip(1)).Concat(BitConverter.GetBytes(MessageVersion).Reverse().Skip(2)).Concat(EncryptedPayload).ToArray();
            }
        }

        /// <summary>
        /// Self-explaining.
        /// 10100, 20100, 10101, 20104 [...]
        /// </summary>
        public int ID
        {
            get
            {
                return this.packetID;
            }
        }

        /// <summary>
        /// 2 bytes nobody has exact info about.
        /// </summary>
        public int MessageVersion
        {
            get
            {
                return this.messageVer;
            }
        }
        /// <summary>
        /// String representation according to the ID.
        /// 10100 => LoginMessage
        /// 10108 => KeepAlive
        /// </summary>
        public string Type
        {
            get
            {
                return this.packetType;
            }
        }

        /// <summary>
        /// Destination. Either client or server.
        /// Admittedly, the Substring method is pretty nasty.
        /// </summary>
        public DataDestination Destination
        {
            get
            {
                return this.destination;
            }
        }

        /// <summary>
        /// Normal payload from the received packet.
        /// </summary>
        public byte[] Payload
        {
            get
            {
                return this.payload;
            }
        }
        /// <summary>
        /// Encrypted payload by <seealso cref="EnDecrypt.EncryptPacket(Packet)"/>
        /// </summary>
        public byte[] EncryptedPayload
        {
            get
            {
                return this.encryptedPayload;
            }
        }

        /// <summary>
        /// Decrypted payload by <seealso cref="EnDecrypt.DecryptPacket(Packet)"/>
        /// </summary>
        public byte[] DecryptedPayload
        {
            get
            {
                return this.decryptedPayload;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Destination: " + Destination);
            sb.AppendLine("ID: " + ID);
            sb.AppendLine("Type: " + Type);
            sb.AppendLine("PayloadLen: " + DecryptedPayload.Length);
            sb.AppendLine("Payload: " + Encoding.UTF8.GetString(DecryptedPayload));
            return sb.ToString();
        }
    }
}