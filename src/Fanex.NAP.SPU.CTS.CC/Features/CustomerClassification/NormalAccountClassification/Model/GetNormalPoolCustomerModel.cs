namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using System.Collections.Generic;

    public class GetNormalPoolCustomerModel
    {
        public GetNormalPoolCustomerModel()
        {
            this.NormalPoolCustomers = new List<GetNormalPoolCustomerEntity>();
            this.ScannedMaxId = 0;
        }

        public IEnumerable<GetNormalPoolCustomerEntity> NormalPoolCustomers { get; set; }

        public long ScannedMaxId { get; set; }
    }
}
