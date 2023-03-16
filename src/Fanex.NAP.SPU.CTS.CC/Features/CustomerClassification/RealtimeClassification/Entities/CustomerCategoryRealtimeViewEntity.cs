namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using System.Collections.Generic;

    public class CustomerCategoryRealtimeViewEntity
    {
        public IEnumerable<CustomerCategoryRealtimeEntity> CustCategoryRealtime { get; set; }

        public IEnumerable<long> CustPADailyScan { get; set; }
    }
}