namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    public class InsertRobotParaEntity
    {
        public int BatchExternalSize { get; set; }

        public int BatchInternalSize { get; set; }

        public string ServiceName { get; set; }

        public bool IsWriteLog { get; set; }

        public string Soon88FeedDataAPI { get; set; }

        public bool IsPushCCToSoon88 { get; set; }

        public bool IsSyncAFCData { get; set; }

        public string AFCInsertAccountURL { get; set; }

        public string AFCUpdateAccountURL { get; set; }

        public string AFCGetAccountURL { get; set; }

        public string AFCInternalKey { get; set; }

        public bool IsFromTW { get; set; }

        public bool IsFromCTS { get; set; }

        public bool IsDirect { get; set; }

        public bool IsFromAI { get; set; }

    }
}