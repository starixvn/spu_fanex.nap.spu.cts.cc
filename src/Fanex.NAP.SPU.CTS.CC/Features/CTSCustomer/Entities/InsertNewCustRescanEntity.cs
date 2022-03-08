namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    public class InsertNewCustRescanEntity
    {
        public int InternalBatchSize { get; set; }

        public int ExternalBatchSize { get; set; }

        public bool IsWriteLog { get; set; }

        public string FeedSoon88API { get; set; }

        public bool IsInsertMissingCust { get; set; }

        public bool IsSyncAFCData { get; set; }

        public string AFCInsertAccountURL { get; set; }

    }
}