namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using System.Collections.Generic;

    public class GetCustomerCategoryBySportModel
    {
        public GetCustomerCategoryBySportModel() 
        {
            CustomerNormals = new List<CustClassificationRealtimeBySportEntity>();
            CustomerPADailyScans = new List<CustClassificationRealtimeBySportEntity>();
        }

        public List<CustClassificationRealtimeBySportEntity> CustomerNormals { get; set; }

        public List<CustClassificationRealtimeBySportEntity> CustomerPADailyScans { get; set; }
    }
}
