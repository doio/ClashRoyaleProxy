using Sodium;

namespace ClashRoyaleProxy
{
    public class ServerState : State
    {
        public KeyPair serverKey;
        public byte[] clientKey, nonce, sessionKey, sharedKey;
    }
}
