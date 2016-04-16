using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyaleProxy
{
    class PacketType
    {
        private static Dictionary<int, string> KnownPackets = new Dictionary<int, string>()
        {
            /* ALL PACKET TYPES! */
            { 10100, "ClientHello" },
            { 10101, "Login" },
            { 10107, "ClientCapabilities" },
            { 10108, "KeepAlive" },
            { 10112, "AuthenticationCheck" },
            { 10113, "SetDeviceToken" },
            { 10116, "ResetAccount" },
            { 10117, "ReportUser" },
            { 10118, "AccountSwitched" },
            { 10121, "UnlockAccount" },
            { 10150, "AppleBillingRequest" },
            { 10151, "GoogleBillingRequest" },
            { 10159, "KunlunBillingRequest" },
            { 10212, "ChangeAvatarName" },
            { 10512, "AskForPlayingGamecenterFriends" },
            { 10513, "AskForPlayingFacebookFriends" },
            { 10905, "InboxOpened" },
            { 12211, "UnbindFacebookAccount" },
            { 12903, "RequestSectorState" },
            { 12904, "SectorCommand" },
            { 12905, "GetCurrentBattleReplayData" },
            { 12951, "SendBattleEvent" },
            { 14101, "GoHome" },
            { 14102, "EndClientTurn" },
            { 14104, "StartMission" },
            { 14105, "HomeLogicStopped" },
            { 14107, "CancelMatchmake" },
            { 14108, "ChangeHomeName" },
            { 14113, "VisitHome" },
            { 14114, "HomeBattleReplay" },
            { 14117, "HomeBattleReplayViewed" },
            { 14120, "AcceptChallenge" },
            { 14123, "CancelChallengeMessage" },
            { 14201, "BindFacebookAccount" },
            { 14212, "BindGamecenterAccount" },
            { 14262, "BindGoogleServiceAccount" },
            { 14301, "CreateAlliance" },
            { 14302, "AskForAllianceData" },
            { 14303, "AskForJoinableAlliancesList" },
            { 14304, "AskForAllianceStream" },
            { 14305, "JoinAlliance" },
            { 14306, "ChangeAllianceMemberRole" },
            { 14307, "KickAllianceMember" },
            { 14308, "LeaveAlliance" },
            { 14310, "DonateAllianceUnit" },
            { 14315, "ChatToAllianceStream" },
            { 14316, "ChangeAllianceSettings"},
            { 14317, "RequestJoinAlliance" },
            { 14318, "SelectSpellsFromCoOpen" },
            { 14319, "OfferChestForCoOpen" },
            { 14321, "RespondToAllianceJoinRequest" },
            { 14322, "SendAllianceInvitation" },
            { 14323, "JoinAllianceUsingInvitation" },
            { 14324, "SearchAlliances" },
            { 14330, "SendAllianceMail" },
            { 14401, "AskForAllianceRankingList" },
            { 14402, "AskForTVContent" },
            { 14403, "AskForAvatarRankingList" },
            { 14404, "AskForAvatarLocalRanking" },
            { 14405, "AskForAvatarStream" },
            { 14406, "AskForBattleReplayStream" },
            { 14408, "AskForLastAvatarTournamentResults" },
            { 14418, "RemoveAvatarStreamEntry" },
            { 14600, "AvatarNameCheckRequest" },
            { 16000, "LogicDeviceLinkCodeStatus" },
            { 20100, "ServerHello" },
            { 20103, "LoginFailed" },
            { 20104, "LoginOk" },
            { 20105, "FriendList" },
            { 20108, "KeepAliveOk" },
            { 20118, "ChatAccountBanStatus" },
            { 20121, "BillingRequestFailed" },
            { 20132, "UnlockAccountOk" },
            { 20133, "UnlockAccountFailed" },
            { 20151, "AppleBillingProcessedByServer" },
            { 20152, "GoogleBillingProcessedByServer" },
            { 20156, "KunlunBillingProcessedByServer" },
            { 20161, "ShutdownStarted" },
            { 20205, "AvatarNameChangeFailed" },
            { 20206, "AvatarOnlineStatusUpdated"},
            { 20207, "AllianceOnlineStatusUpdated" },
            { 20225, "BattleResult" },
            { 20300, "AvatarNahmeCheckResponse" },
            { 20801, "OpponentLeftMatchNotification" },
            { 20802, "OpponentRejoinsMatchNotification" },
            { 21902, "SectorHearbeat" },
            { 21903, "SectorState" },
            { 22952, "BattleEvent" },
            { 22957, "PvpMatchmakeNotification" },
            { 24101, "OwnHomeData" },
            { 24102, "OwnAvatarData" },
            { 24104, "OutOfSync" },
            { 24106, "StopHomeLogic" },
            { 24107, "MatchmakeInfo" },
            { 24108, "MatchmakeFailed" },
            { 24111, "AvailableServerCommand" },
            { 24112, "UdpConnectionInfo" },
            { 24113, "VisitedHomeData" },
            { 24114, "HomeBattleReplay" },
            { 24115, "ServerError" },
            { 24116, "HomeBattleReplayFailed" },
            { 24121, "ChallengeFailed" },
            { 24124, "CancelChallengeDone" },
            { 24125, "CancelMatchmakeDone" },
            { 24201, "FacebookAccountBound" },
            { 24202, "FacebookAccountAlreadyBound" },
            { 24212, "GamecenterAccountAlreadyBound" },
            { 24213, "FacebookAccountUnbound" },
            { 24261, "GoogleServiceAccountBound" },
            { 24262, "GoogleServiceAccountAlreadyBound" },
            { 24301, "AllianceData" },
            { 24302, "AllianceJoinFailed" },
            { 24303, "AllianceJoinOk" },
            { 24304, "JoinableAllianceList" },
            { 24305, "AllianceLeaveOk" },
            { 24306, "ChangeAllianceMemberRoleOk" },
            { 24307, "KickAllianceMemberOk" },
            { 24308, "AllianceMember" },
            { 24309, "AllianceMemberRemoved" },
            { 24310, "AllianceList" },
            { 24311, "AllianceStream" },
            { 24312, "AllianceStreamEntry" },
            { 24318, "AllianceStreamEntryRemoved" },
            { 24319, "AllianceJoinRequestOk" },
            { 24320, "AllianceJoinRequestFailed" },
            { 24321, "AllianceInvitationSendFailed" },
            { 24322, "AllianceInvitationSentOk" },
            { 24324, "AllianceFullEntryUpdate" },
            { 24332, "AllianceCreateFailed" },
            { 24333, "AllianceChangeFailed" },
            { 24401, "AllianceRankingList" },
            { 24402, "AllianceLocalRankingList" },
            { 24403, "AvatarRankingList" },
            { 24404, "AvatarLocalRankingList" },
            { 24405, "RoyalTVContent" },
            { 24407, "LastAvatarTournamentResults" },
            { 24411, "AvatarStream" },
            { 24412, "AvatarStreamEntry" },
            { 24413, "BattleReportStream" },
            { 24418, "AvatarStreamEntryRemoved" },
            { 24445, "InboxList" },
            { 24447, "InboxCount" },
            { 25892, "Disconnected" },
            { 26002, "LogicDeviceLinkCodeResponse" },
            { 26003, "LogicDeviceLinkNewDeviceLinked" },
            { 26004, "LogicDeviceLinkCodeDeactivated" },
            { 26005, "LogicDeviceLinkResponse" },
            { 26007, "LogicDeviceLinkDone" },
            { 26008, "LogicDeviceLinkError" },
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
