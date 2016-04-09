using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyaleProxy
{
    class PacketType
    {
        private static Dictionary<int, string> KnownPackets = new Dictionary<int, string>
        {
            { 10100, "Session" },
            { 10101, "Login" },
            { 10108, "KeepAlive" },
            { 20100, "SessionOk" },
            { 20103, "LoginFailed" },
            { 20104, "LoginOk" },
            { 20108, "KeepAliveOk" },
            { 24101, "OwnHomeData" },
            { 24104, "OutOfSync" },
            { 24114, "HomeBattleReplay" },
            { 24115, "ServerError" },
            { 14124, "CancelMatchmaking" },
            { 24124, "CancelMatchmakingDone" },
            { 14102, "GameCommand" },
            { 24405, "TVRoyaleReplayData" },
            { 10107, "EndClientTurn" },
            { 10905, "RequestNews" },
            { 24445, "NewsData" },
            { 14312, "SendGlobalChatLine" },
            { 24312, "GlobalChatLine" },
            { 24111, "ChestLootData" },
            { 10212, "NameChange" }
        };
        
        public static string GetPacketTypeByID(int messageID)
        {
            if (KnownPackets.ContainsKey(messageID))
            {
                string ret = String.Empty;
                KnownPackets.TryGetValue(messageID, out ret);
                return ret;
            }
            return "Yet unknown :(";
        }

        public static int GetPacketIDByType(string type)
        {
            return KnownPackets.FirstOrDefault(x => x.Value == type).Key;
        }
    }
}
