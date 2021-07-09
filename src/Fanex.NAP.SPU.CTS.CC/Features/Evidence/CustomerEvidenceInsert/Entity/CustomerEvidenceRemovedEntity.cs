namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    public class CustomerEvidenceRemovedEntity
    {
        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public int SubscriberId { get; set; }

        public int EvidenceId { get; set; }

        public int ActionType { get; set; }
    }
}