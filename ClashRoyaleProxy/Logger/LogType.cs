using System;

namespace ClashRoyaleProxy
{
    enum LogType
    {
        INFO, // A normal text (i.e. "Proxy started")
        WARNING, // A warning (i.e. 2 running proxys)
        PACKET, // A client/server packet (i.e. KeepAlive)
        EXCEPTION // An exception (i.e. NullReferenceException)
    }
}
