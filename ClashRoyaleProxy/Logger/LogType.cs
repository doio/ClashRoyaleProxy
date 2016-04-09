using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyaleProxy
{
    enum LogType
    {
        INFO, // A normal text (i.e. "Proxy started")
        WARNING, // A warning (i.e. 2 running proxys)
        CLIENT_PACKET, // A client-packet (i.e. "KeepAlive")
        SERVER_PACKET, // A server-packet (i.e. "LoginOk")
        EXCEPTION // An exception (i.e. NullReferenceException)
    }
}
