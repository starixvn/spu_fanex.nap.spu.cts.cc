namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence
{
    using System;

    public class CustEvidenceEntity
    {
        public long CustId { get; set; }

        public long CTSCustId { get; set; }

        public int SubscriberId { get; set; }

        public int EvidenceId { get; set; }

        public Int64 CreatedBy { get; set; }

        public int ActionType { get; set; }
    }
}