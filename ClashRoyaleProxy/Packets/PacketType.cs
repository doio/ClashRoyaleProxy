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
            #region Client packets
            { 10100, "Session" },
            { 10101, "Login" },
            { 10107, "EndClientTurn" },
            { 10108, "KeepAlive" },
            { 10212, "NameChange" },
            { 10905, "RequestNews" },
            { 14102, "GameOpCommand" },
            { 14113, "AskAvatarProfileData" },
            { 14124, "CancelMatchmaking" },   
            { 14312, "SendGlobalChatLine" },
            #endregion

            #region Server packets
            { 20100, "SessionOk" },
            { 20103, "LoginFailed" },
            { 20104, "LoginOk" },
            { 20108, "KeepAliveOk" },
            { 24101, "OwnHomeData" },
            { 24104, "OutOfSync" },
            { 24111, "ChestLootData" },
            { 24113, "AvatarProfileData" },
            { 24114, "HomeBattleReplay" },
            { 24115, "ServerError" },
            { 24124, "CancelMatchmakingDone" },
            { 24312, "GlobalChatLine" },
            { 24405, "TVRoyaleReplayData" },
            { 24445, "NewsData" },
            #endregion
        };

        /// <summary>
        /// Gets the packet type according to the ID
        /// </summary>
        public static string GetPacketTypeByID(int messageID)
        {
            if (KnownPackets.ContainsKey(messageID))
            {
                string ret = String.Empty;
                KnownPackets.TryGetValue(messageID, out ret);
                return ret;
            }
            return "Unknown packet";
        }

        /// <summary>
        /// Gets the packet ID according to the type
        /// </summary>
        public static int GetPacketIDByType(string type)
        {
            return KnownPackets.FirstOrDefault(x => x.Value == type).Key;
        }
    }
}
