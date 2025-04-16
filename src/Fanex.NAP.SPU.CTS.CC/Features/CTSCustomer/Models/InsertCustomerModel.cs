namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models
{
    using System.Collections.Generic;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;

    public class InsertCustomerModel
    {
        public List<CustomerMainDBEntity> CustomerInfos { get; set; }

        public List<int> MissingCustIds { get; set; }

        public long? LastInsertCustID { get; set; }
    }
}