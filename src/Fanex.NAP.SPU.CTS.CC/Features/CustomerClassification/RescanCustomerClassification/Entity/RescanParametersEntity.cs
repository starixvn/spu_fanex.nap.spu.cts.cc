namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RescanParametersEntity
    {
        public IEnumerable<long> CustIds { get; set; }

        public bool IsRescanPA { get; set; }

        public bool IsPushCCToSoon88 { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }

        public DangerLevel DangerLevelType { get; set; }

        public FunctionId FunctionId { get; set; }

        public int InternalBatchSize { get; set; }

        public int ExternalBatchSize { get; set; }

        public string ServiceName { get; set; }

        public bool IsWriteLog { get; set; }

        public string FeedSoon88API { get; set; }

        public bool IsSyncAFCData { get; set; }

        public string AFCInsertAccountURL { get; set; }

        public FunctionId FuncId_CheckRobotUser
        {
            get
            {
                switch (FunctionId)
                {
                    case FunctionId.InsertNewCustJob:
                        return FunctionId.InsertNewCus_CheckRobotUser;

                    case FunctionId.RemoveVIPWeb:
                        return FunctionId.RemoveVVIP_CheckRobotUser;

                    case FunctionId.RemoveSpecialCCWeb:
                        return FunctionId.RemoveSpecial_CheckRobotUser;

                    case FunctionId.RemovePAAPI:
                        return FunctionId.RemovePA_CheckRobotUser;

                    case FunctionId.RemoveFlaggedEvidenceWeb:
                        return FunctionId.RemoveFlaggedEvi_CheckRobotUser;

                    case FunctionId.RemoveMultiEvidencesWeb:
                        return FunctionId.RemoveMultiEvi_CheckRobotUser;

                    case FunctionId.ExcludeTaggingAssPAWeb:
                        return FunctionId.ExcludeTaggingAss_CheckRobotUser;

                    default:
                        return 0;
                }
            }
        }
    }
}