namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport
{
    using System.Collections.Generic;

    public class GetNewPlaceBetCustomerModel
    {
        public GetNewPlaceBetCustomerModel()
        {
            this.CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>();
            this.ScannedMaxId = 0;
        }

        public IEnumerable<CustClassificationRealtimeBySportEntity> CustomerNewPlaceBets { get; set; }

        public long ScannedMaxId { get; set; }
    }
}
