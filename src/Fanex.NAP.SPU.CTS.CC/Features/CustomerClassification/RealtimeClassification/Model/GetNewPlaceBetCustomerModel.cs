namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using System.Collections.Generic;

    public class GetNewPlaceBetCustomerModel
    {
        public GetNewPlaceBetCustomerModel ()
        {
            this.CustomerNewPlaceBets = new List<GetNewPlaceBetCustomerEntity>();
            this.ScannedMaxId = 0;
        }

        public IEnumerable<GetNewPlaceBetCustomerEntity> CustomerNewPlaceBets { get; set; }

        public long ScannedMaxId { get; set; }
    }
}
