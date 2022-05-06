namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities
{    
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;    
    using System.Collections.Generic;
    using System.Linq;

    public class CustInsertedInfoModel
    {
        public CustInsertedInfoModel()
        {
            InsertCustomer = Enumerable.Empty<InsertCustomerModel>();

            CustInfoVVip = Enumerable.Empty<CustInfoVVipModel>();
        }

        public IEnumerable<InsertCustomerModel> InsertCustomer { get; set; }

        public IEnumerable<CustInfoVVipModel> CustInfoVVip { get; set; }

    }
}
