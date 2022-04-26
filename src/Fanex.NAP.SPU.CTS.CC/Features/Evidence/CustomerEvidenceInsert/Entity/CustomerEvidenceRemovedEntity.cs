namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert
{
    using System;

    public class CustomerEvidenceRemovedEntity
    {
        public long CTSCustId { get; set; }

        public long CustId { get; set; }

        public int SubscriberId { get; set; }

        public int EvidenceId { get; set; }

        public Int64 CreatedBy { get; set; }

        public int ActionType { get; set; }
    }
}