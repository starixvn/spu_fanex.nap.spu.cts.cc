namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    public class CustomerEvidenceFailedToProcessEntity
    {
        public string FileKey { get; set; }

        public long CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public int EvidenceID { get; set; }

        public string ErrorContent { get; set; }
    }
}