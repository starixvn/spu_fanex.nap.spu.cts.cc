namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RealtimeCheckChangeModel
    {
        public RealtimeCheckChangeModel() 
        {
            CustInfos = Enumerable.Empty<RealtimeCheckChangeEntities>();
        }

        public IEnumerable<RealtimeCheckChangeEntities> CustInfos { get; set; }

        public DateTime? NextScannedDateTime { get; set; }
    }
}
