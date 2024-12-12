using System.ComponentModel;

namespace Fanex.NAP.SPU.CTS.Shares
{
    public static class Constants
    {
        public const int RetryPushCCToExternal = 3;

        public const int InternalRetry = 3;

        public const string FeedSoon88_Salt = "!%HGDwweHqw#";

        public const string CONVERT_TO_SQL_DATETIME = "yyyy-MM-dd HH:mm:ss.fff";

        public const string CONVERT_TO_SQL_DATE = "yyyy-MM-dd";
        public const string CONVERT_TO_DATETIME = "dd/MM/yyyy HH:mm:ss";
        public const string CONVERT_TO_DATETIME_ddMMMyyyy = "dd MMM, yyyy";
        public const string CONVERT_TO_DATETIME_MMMddyyyy = "MMM dd, yyyy";
        public const string FormatDecimalToString_2Places = "0.00";
        public const string FormatDecimalToString_Thousands = "#,##0";
        public const string FormatDecimalToString_Thousands2Places = "#,##0.00";
        public const string FormatDecimalToString_Thousands3Places = "#,##0.##";

        public const int SabaDangerLevel_SleepTimes = 5000;

        public const int RobotUsers_SleepTimes = 300;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2148:Underscores should be used to make large numbers readable", Justification = "<Pending>")]
        public const int StarixITId = 10278938;

        public const string MicrosoftTeams_GroupId_Local = "19:00af2ff331b64d39adf13b428aa49c6e@thread.tacv2";

        public const string MicrosoftTeams_JobMonitor_GroupId_Pro = "19:3537f2795b2a49aabc926b3275583615@thread.tacv2";
        public const string DangerousScoreRemark = "Detection Date: {0} - Dangerous Score: {1}%";
        public const string TWInitialGBMissSport = "Soccer/Basketball";
        public const string TWInitialSmartRemark = "Detection Date: {0} - Probability Threshold: {1}";
        public const string TWTWInitialGBRemark = "Detection Date: {0} - GB Bet Count: {1} - Sport: {2}";

        public const string CATEGORYTYPEPARENT_CATEID_ROBOTUSER = "CONST_CATEID_ROBOTUSER";

        public const string DataInfo = "Data Info";
        public const string ExceptionInfo = "Exception Info";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum CTS_SystemParameters
        {
            TVS_LastScanDateTime = 2,
            RobotAccountLastScannedTime = 3,
            MatchMonitorLastScannedTrans = 20,
            JobMonitor_NewCustomerLastScannedTime = 67,
            ReactivatedCustomerLastScannedDate = 76,
            InactiveNewMember_LastCategoryID = 80,
            InactiveNewMember_LastCustID = 81,
            DailyNormalClassificationLastCategoryID = 85,
            DailyNormalClassificationLastCustID = 86,
            DailyProblemClassificationLastScannedTime = 88,
            NewCTSCustomerClassificationLastCustID = 92,
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
            NormalTaggingClassificationLastCategoryID = 144,
            NormalTaggingClassificationLastCustID = 145,
            CustomerNullUsername2_LastCTSCustID = 146,
            UnauthorizedLogin_LastScanCustID_Credit = 147,
            UnauthorizedLogin_LastScanCustID_Licensee = 148,
            UnauthorizedLogin_LastScanDate_Credit = 149,
            UnauthorizedLogin_LastScanDate_Licensee = 150,
            JobMonitoring_IncorrectData_LastScanID = 151,
            JobMonitoring_NewCustomer_LastCTSCustID = 152,
            DangerousScoreClassification_Credit_LastScannedDate = 153,
            DangerousScoreClassification_Credit_LastCustID = 154,
            DangerousScoreClassification_Deposit_LastScannedDate = 156,
            DangerousScoreClassification_Deposit_LastCustID = 157,
            TWInitialSmartClassification_LastCustID = 159,
            TWInitialSmartClassification_LastScanDate = 160,
            DailyProbation_LastCategoryID = 166,
            DailyProbation_LastCustID = 167,
            DailyProbation_BySport_LastCustID = 169,
            SabaDangerLevel_Agency_LastHistoryID = 181,
            NormalDangerousAssociationCheck_LastCTSAssDevID = 183,
            NormalDangerousAssociationScan_LastScannedID = 184,
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum Main_SystemParameters
        {
            TWGroupBetting_LastTransSec = 1,
            TWGroupBetting_SkipTransCount = 2,
            TWGroupBetting_ScanTicket_LastSequenceId = 3,
            TWGroupBetting_CountTicket_BatchSize = 4,
            TWGroupBetting_CountTicket_LastSequenceId = 5,
            TWGroupBetting_CountTicket_Status = 6,
        }

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
            ExcludeTaggingAssPAWeb = 10,
            InactivateNewCustomer = 11,
            ProblemDailyJob = 12,
            RobotUserJob = 23,
            InsertNewCus_CheckRobotUser = 34,
            RemoveVVIP_CheckRobotUser = 35,
            RemoveSpecial_CheckRobotUser = 36,
            InsertPAJob = 41,
            InsertPAM = 43,
            TWProblemAccountInsert = 44,
            ScanAssociatedAccount = 51,
            DownlineCustomerClassification = 52,
            TVSProblemAccountInsert = 53,
            ClassifyGBMatchMonitor = 54,
            NormalTaggingJob = 100,
            DetectUnauthorizedLoginCust = 101,
            DailyDangerousClassification = 102,
            TWInitialGBClassification = 103,
            TWInitialSmartClassification = 104,
            NormalDangerousTagging = 105,
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum BySport_FunctionId
        {
            InsertNewCustJob = 4
        };

        public enum RescanNormalType
        {
            Manual = 4,
            Auto_Customer = 5,
            Auto_AfftectedDownline = 6,
            Auto_NormalTaggingScan = 7,
            Auto_InactiveCustomer = 8,
            Auto_DangerousAssociation = 9,
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum BySport_RescanNormalType
        {
            Service = 1,
            Manual = 2
        }

        public enum FromFlow
        {
            CTSWEB = 1,
            CTSAPI = 2,
            CTSServices = 3,
            ManualTrigger = 4
        };

        public enum QueryType
        {
            GetByRange = 1,
            GetByCustId = 2
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum DownlineClassification_ActionType
        {
            InsertVVIP = 1,
            RemoveVVIP = 2,
            InsertPA = 3,
            RemovePA = 4,
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum JobMonitor_IssueType
        {
            NewCustomer_NullCategory = 1,
            NewCustomer_Missing = 2,
            NewCustomer_IncorrectCustInfo = 3,
            Realtime_NewPlaceBetInactiveCategory = 100,
            Realtime_NullCategory = 101,
            Classification_GeneralPoolStatus = 200,
            Classification_BySportPoolStatus = 201,
            Classification_MissingTVSProblemAccount = 202,
            DCSTransaction_LastLoginProcessStatus = 500
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum CustomerMonitor_QueryType
        {
            MissingCustomer = 1,
            IncorrectCustInfo = 2,
        };

        public enum InvalidCategory
        {
            Invalid_Inactive = 2,
            Invalid_Null = 3
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2342:Enumeration types should comply with a naming convention", Justification = "<Pending>")]
        public enum NormalClasification_TaggingType
        {
            NotExist = 0,
            Exist = 1,
            ExistOnly = 2,
        };

        public enum CCInputFlow
        {
            [Description("VVIP")]
            VVIP = 1,
            [Description("SpecialCC")]
            SpecialCC = 2,
            [Description("LicVIP")]
            LicVIP = 3,
            [Description("LicBA")]
            LicBA = 4,
            [Description("InsertPACategory")]
            InsertPACategory = 5,
            [Description("InsertPAReason")]
            InsertPAReason = 6,
            [Description("Potential")]
            Potential = 8,
            [Description("Normal")]
            Normal = 9,
            [Description("RescanPACategory")]
            RescanPACategory = 115,
            [Description("RescanPotential")]
            RescanPotential = 118,
            [Description("RescanNormal")]
            RescanNormal = 9,
            [Description("RemoveVVIP")]
            RemoveVVIP = 221,
            [Description("RemoveSpecialCC")]
            RemoveSpecialCC = 222,
            [Description("RemoveLicVIP")]
            RemoveLicVIP = 223,
            [Description("RemoveLicBA")]
            RemoveLicBA = 224,
            [Description("RemovePACategory")]
            RemovePACategory = 225,
            [Description("BySportInsertSpecialCC")]
            BySportInsertSpecialCC = 332,
            [Description("BySportInsertNormal")]
            BySportInsertNormal = 339,
            [Description("BySportRemoveSpecialCC")]
            BySportRemoveSpecialCC = 442
        }

        public enum RoleGroupType
        {
            Member = 1,
            Agency = 2
        }
    }
}