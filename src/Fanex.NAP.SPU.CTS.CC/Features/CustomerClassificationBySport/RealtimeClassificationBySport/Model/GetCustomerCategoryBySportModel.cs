namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using System.Collections.Generic;

    public class GetCustomerCategoryBySportModel
    {
        public GetCustomerCategoryBySportModel() 
        {
            CustomerNormals = new List<long>();
            CustomerPADailyScans = new List<long>();
        }

        public List<long> CustomerNormals { get; set; }

        public List<long> CustomerPADailyScans { get; set; }
    }
}
