namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    using System.Collections.Generic;

    public class InsertCustomerViewModel
    {
        public List<CustomerMainDBModel> CustomerInfos { get; set; }

        public List<int> MissingCustIds { get; set; }

        public long? LastInsertCustID { get; set; }
    }
}