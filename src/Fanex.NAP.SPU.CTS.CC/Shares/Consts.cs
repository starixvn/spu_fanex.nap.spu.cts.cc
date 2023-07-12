namespace Fanex.NAP.SPU.CTS.Shares
{
    public static class Constants
    {
        public const int RetryPushCCToExternal = 3;

        public const int InternalRetry = 3;

        public const string FeedSoon88_Salt = "!%HGDwweHqw#";

        public const string CONVERT_TO_SQL_DATETIME = "yyyy-MM-dd HH:mm:ss.fff";

        public const int SabaDangerLevel_SleepTimes = 5000;

        public const int RobotUsers_SleepTimes = 300;

        public const int StarixITId = 10278938;

        public const string Telegram_GroupId_Local = "-869606030";

        public const string Telegram_JobMonitor_GroupId_Pro = "-845108430";

        public enum MatchMonitorQueryTypes
        {
            Service = 1,
            Report = 2,
        };

        public enum SystemParameters
        {
            TVS_LastScanDateTime = 2,
            RobotAccountLastScannedTime = 3,
            MatchMonitorLastScannedTrans = 20,
            JobMonitor_NewCustomerLastScannedTime = 67,
            ReactivatedCustomerLastScannedDate = 76,
            TWGroupBetting_LastTransSec = 77,
            TWGroupBetting_SkipTransCount = 78,
            TWGroupBetting_ScanTicket_LastSequenceId = 90,
            InactiveNewMember_LastCategoryID = 80,
            InactiveNewMember_LastCustID = 81,
            DailyNormalClassificationLastCategoryID = 85,
            DailyNormalClassificationLastCustID = 86,
            DailyProblemClassificationLastScannedTime = 88,
            NewCTSCustomerClassificationLastCustID = 92,
            //RobotDetectionLastModifiedDate = 94,
            RobotDetection_LastID = 94,
            AssociatedGroupLastGroupID = 98,
            AssociatedGroupLastCTSCustID = 99,
            SabaDangerLevel_LastHistoryID = 102,
            SabaDangerLevel_LastCategoryID = 103,
            SabaDangerLevel_LastCustID = 104,
            DailyNormalClassificationBySportLastCategoryID = 114,
            DailyNormalClassificationBySportLastCustID = 115,
            JobMonitor_RealtimeLastScannedTime = 130,
            JobMonitor_NewCustomerLastScannedCustID = 131,
            JobMonitor_NewCustomerLastScannedCustSubID = 132,
            TWRobotImperva_LastLogID = 133,
			//TWRobotImperva_LastTime = 134 // TWRobotImperva_LastCreateTime
            TWRobotImperva_LastID = 134,
            TWGroupBetting_CountTicket_BatchSize = 135,
            TWGroupBetting_CountTicket_LastSequenceId = 136,
            TWGroupBetting_CountTicket_Status = 137
        };

        public enum Reason
        {          
            TVSRobot36 = 36,
            TVSRobot105 = 105,
            ABBet32 = 32,
            ABBet104 = 104
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
            //RemovePAAPI = 7,
            //RemoveFlaggedEvidenceWeb = 8,
            //RemoveMultiEvidencesWeb = 9,
            ExcludeTaggingAssPAWeb = 10,
            InactivateNewCustomer = 11,
            ProblemDailyJob = 12,
            //InsertEvidenceJob = 13,
            //PlaceSingleEvidence = 14,
            //PlaceEvidenceForMultiCustAPI = 21,
            //PlaceMultiEvidencesForMultiCustAPI = 22,
            RobotUserJob = 23,
            //InsertNewCus_CheckPA = 24,
            //RemoveVVIP_CheckPA = 25,
            //RemoveSpecial_CheckPA = 26,
            //RemovePA_CheckPA = 27,
            //RemoveFlaggedEvidence_CheckPA = 28,
            //RemoveMultiEvidences_CheckPA = 29,
            //ExcludeTaggingByAss_CheckPA = 30,
            InsertNewCus_CheckRobotUser = 34,
            RemoveVVIP_CheckRobotUser = 35,
            RemoveSpecial_CheckRobotUser = 36,
            //RemovePA_CheckRobotUser = 37,
            //RemoveFlaggedEvi_CheckRobotUser = 38,
            //RemoveMultiEvi_CheckRobotUser = 39,
            //ExcludeTaggingAss_CheckRobotUser = 40,
            InsertPAJob = 41,
            //UpdateCategory = 42,
            InsertPAM = 43,
            TWProblemAccountInsert = 44,
            ScanAssociatedAccount = 51,
            DownlineCustomerClassification = 52,
            TVSProblemAccountInsert = 53,
            ClassifyGBMatchMonitor = 54       
        };

        public enum RescanNormalType
        {
            Manual = 4,
            Auto_Customer = 5,
            Auto_AfftectedDownline = 6
        }

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

        public enum VVIP_FromAction
        {
            WebSite = 1,
            Service = 2,
            ServiceInsertNew = 3
        }

        public enum DownlineClassification_ActionType
        {
            InsertVVIP = 1,
            RemoveVVIP = 2,
            InsertPA = 3,
            RemovePA = 4,
        }

        public enum InsertPA_ByType
        {
            Category = 1,
            Reason = 2,
        };

        public enum JobMonitorType
        {
            NewCustomer_NullCategory = 1,
            Realtime_NewPlaceBetInactiveCategory = 2,
            Realtime_NullCategory = 3,
            NewCustomer_Missing = 4,
        };

        public enum InvalidCategory
        {
            Invalid_Inactive = 2,
            Invalid_Null = 3            
        };
    }
}