namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport
{   
    using System.Collections.Generic;

    public class GetNormalPoolCustomerBySportModel
    {
        public GetNormalPoolCustomerBySportModel()
        {
            this.NormalPoolCustomers = new List<GetNormalPoolCustomerBySportEntity>();
            this.ScannedMaxId = 0;
        }

        public IEnumerable<GetNormalPoolCustomerBySportEntity> NormalPoolCustomers { get; set; }

        public long ScannedMaxId { get; set; }
    }
}
