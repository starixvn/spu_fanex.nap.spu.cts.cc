namespace Fanex.NAP.SPU.CTS.Shares
{
    public static class Constants
    {
        public const int RetryPushCCToExternal = 3;

        public const int InternalRetry = 3;

        public const string InitialProblemAccounts = "0";

        public const string ProblemAccountByDateTime = "";

        public const string FeedSoon88_Salt = "!%HGDwweHqw#";

        public const string CONVERT_TO_SQL_DATETIME = "yyyy-MM-dd HH:mm:ss";

        public const int Realtime_SleepTimes = 3000;

        public const int RemoveEvidences_ClearActionId = 3;

        public const int ImportEvidence_StarixITId = 10278938;

        public const int ImportEvidence_SleepTimes = 2000;

        public enum MatchMonitorQueryTypes
        {
            Service = 1,
            Report = 2,
        };

        public enum SystemParameters
        {
            ProblemAccountLastScannedTime = 2,
            RobotAccountLastScannedTime = 3,
            MatchMonitorLastScannedTrans = 20
        };

        public enum DangerLevel
        {
            Original = 1,
            ABI = 2,
            Other = 3
        }

        public enum Category
        {
            Newmember = 201,
            NormalMember = 202,
            SmartPunter = 203,
            HighRishPunter = 204,
            VIP = 205,
            Probation = 206,
            VVIP = 213,
            Robot = 61
        };

        public enum CustomerRole
        {
            Member = 1,
            Agent = 2,
            Master = 3,
            Super = 4
        };

        public enum FlowTypeRescanCC
        {
            NewMember,
            RemoveMultipleEvidence,
            Other
        };

        public enum UpdateCustomer
        {
            FromCustomer = 1,
            FromCustInfo = 2
        };

        public enum EvidenceActionType
        {
            Insert = 0,
            Remove = 2
        };

        public enum FunctionId
        {
            Realtime = 1,
            Probation = 2,
            Daily = 3,
            InsertNewCust = 4,
            RemoveVIP = 5,
            RemoveSpecialCC = 6,
            RemovePA = 7,
            RemoveFlaggedEvidence = 8,
            RemoveMultiEvidences = 9,
            ExcludeTaggingAssPA = 10,
        };
    }
}