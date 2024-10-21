namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownlineCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CustClassificationFromQueueModel
    {
        public CustClassificationFromQueueModel()
        {
            this.MemberQueue = Enumerable.Empty<CustClassificationFromQueueEntity>();
            this.AgencyQueue = Enumerable.Empty<CustClassificationFromQueueEntity>();
        }

        public IEnumerable<CustClassificationFromQueueEntity> MemberQueue { get; set; }

        public IEnumerable<CustClassificationFromQueueEntity> AgencyQueue { get; set; }

        public int ActionType { get; set; }

        public int UplineRoleID { get; set; }

        public UInt64 QueueID { get; set; }

        public Int64 LastDownlineCTSCustID { get; set; }

        public int InternalBatchSize { get; set; }

        public int ExternalBatchSize { get; set; }
    }
}
