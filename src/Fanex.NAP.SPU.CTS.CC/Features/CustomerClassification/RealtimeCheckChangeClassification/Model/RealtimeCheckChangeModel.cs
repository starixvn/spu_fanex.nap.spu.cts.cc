namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RealtimeCheckChangeModel
    {
        public RealtimeCheckChangeModel() 
        {
            CustInfos = Enumerable.Empty<RealtimeCheckChangeEntities>();
            CustInfosAgency = Enumerable.Empty<RealtimeCheckChangeAgencyEntities>();
        }

        public IEnumerable<RealtimeCheckChangeEntities> CustInfos { get; set; }

        public IEnumerable<RealtimeCheckChangeAgencyEntities> CustInfosAgency { get; set; }

        public DateTime? NextScannedDateTime1 { get; set; }

        public DateTime? NextScannedDateTime2 { get; set; }

        public DateTime? NextScannedDateTime3 { get; set; }

        public DateTime? NextScannedDateTime4 { get; set; }
    }
}
