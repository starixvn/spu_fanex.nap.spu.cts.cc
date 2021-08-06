namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Entities
{
    using System.Collections.Generic;

    public class UpdateCustomerModel
    {
        public List<CustomerMainDBModel> CustomerInfos { get; set; }

        public long? LastInsertCustID { get; set; }
    }
}