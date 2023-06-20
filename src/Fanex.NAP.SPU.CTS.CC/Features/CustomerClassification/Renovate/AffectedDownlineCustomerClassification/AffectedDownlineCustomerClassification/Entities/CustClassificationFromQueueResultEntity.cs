namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.AffectedDownline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CustClassificationFromQueueResultEntity
    {
        public CustClassificationFromQueueResultEntity()
        {
            this.CustomerClassificationQueue = Enumerable.Empty<CustClassificationFromQueueEntity>();
        }

        public IEnumerable<CustClassificationFromQueueEntity> CustomerClassificationQueue { get; set; }

        public int ActionType { get; set; }

        public int UplineRoleID { get; set; }

        public UInt64 QueueID { get; set; }

        public Int64 LastDownlineCTSCustID { get; set; }
    }
}