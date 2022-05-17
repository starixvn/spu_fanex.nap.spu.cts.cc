namespace Fanex.NAP.SPU.CTS.Shares
{
    public static class Constants
    {
        public const int RetryPushCCToExternal = 3;

        public const int InternalRetry = 3;

        public const string FeedSoon88_Salt = "!%HGDwweHqw#";

        public const string CONVERT_TO_SQL_DATETIME = "yyyy-MM-dd HH:mm:ss";

        public const int Realtime_SleepTimes = 2000;

        public const int RobotUsers_SleepTimes = 300;

        public const int StarixITId = 10278938;

        public const int ProblemAccountLosing_BatchSize = 2000;

        public enum MatchMonitorQueryTypes
        {
            Service = 1,
            Report = 2,
        };

        public enum SystemParameters
        {
            ProblemAccountLastScannedTime = 2,
            RobotAccountLastScannedTime = 3,
            MatchMonitorLastScannedTrans = 20,
            InsertNewCustomerMonitoring = 67,
            RobotAIClassificationLastScannedID = 69,
            ReactivatedCustomerLastScannedDate = 76,
            TWGroupBetting_LastScanTime = 77,
            TWGroupBetting_LastTimeCountTotalTicket = 78,
            TWGroupBetting_LastTransID = 90,
            InactiveNewMember_LastCategoryID = 80,
            InactiveNewMember_LastCustID = 81,
            DailyNormalClassificationLastCategoryID = 85,
            DailyNormalClassificationLastCustID = 86,
            DailyProblemClassificationLastCategoryID = 88,
            DailyProblemClassificationLastCustID = 89
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
            ForNewCustomer = 0,
            FromCustomer = 1,
            FromCustInfo = 2,
            FromProductStatus = 3
        };

        public enum FunctionId
        {
            RealtimeJob = 1,
            ProbationJob = 2,
            NormalDailyJob = 3,
            InsertNewCustJob = 4,
            RemoveVIPWeb = 5,
            RemoveSpecialCCWeb = 6,
            RemovePAAPI = 7,
            RemoveFlaggedEvidenceWeb = 8,
            RemoveMultiEvidencesWeb = 9,
            ExcludeTaggingAssPAWeb = 10,
            InactivateNewCustomer = 11,
            ProblemDailyJob = 12,
            InsertEvidenceJob = 13,
            PlaceSingleEvidence = 14,
            PlaceEvidenceForMultiCustAPI = 21,
            PlaceMultiEvidencesForMultiCustAPI = 22,
            RobotUserJob = 23,
            InsertNewCus_CheckPA = 24,
            RemoveVVIP_CheckPA = 25,
            RemoveSpecial_CheckPA = 26,
            RemovePA_CheckPA = 27,
            RemoveFlaggedEvidence_CheckPA = 28,
            RemoveMultiEvidences_CheckPA = 29,
            ExcludeTaggingByAss_CheckPA = 30,
            InsertNewCus_CheckRobotUser = 34,
            RemoveVVIP_CheckRobotUser = 35,
            RemoveSpecial_CheckRobotUser = 36,
            RemovePA_CheckRobotUser = 37,
            RemoveFlaggedEvi_CheckRobotUser = 38,
            RemoveMultiEvi_CheckRobotUser = 39,
            ExcludeTaggingAss_CheckRobotUser = 40,
            InsertPAJob = 41,
            UpdateCategory = 42
        };

        public enum HttpStatus
        {
            Unauthorized = 401,
            BadRequest = 400,
            NotFound = 404,
            MethodNotAllowed = 405,
            OK = 200,
            Created = 201,
            ServerUnavailable = 503,
            InternalServerError = 500
        };

        public enum FromFlow
        {
            CTSWEB = 1,
            CTSAPI = 2,
            CTSServices = 3,
            AFCSerivce = 4,
            ManualTrigger = 5
        };

        public enum QueryType
        {
            GetByRange = 1,
            GetByCustId = 2
        };

        public enum FromAction
        {
            WebSite = 1,
            Service = 2,
            ServiceInsertNew = 3
        }

        public enum VVIPActionType
        {
            Insert = 1,
            Remove = 2
        }
    }
}