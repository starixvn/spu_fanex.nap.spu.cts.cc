namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeClassification
{
    using System.Collections.Generic;

    public class GetCustomerCategoryRealtimeModel
    {
        public GetCustomerCategoryRealtimeModel()
        {
            this.CustomerCategory = new List<long>();
            this.CustomerPADailyScan = new List<long>();
        }

        public List<long> CustomerCategory { get; set; }

        public List<long> CustomerPADailyScan { get; set; }
    }
}
