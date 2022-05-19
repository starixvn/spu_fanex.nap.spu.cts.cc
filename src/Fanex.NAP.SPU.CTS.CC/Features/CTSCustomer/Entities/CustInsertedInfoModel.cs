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

        public IEnumerable<InsertCustomerModel> MergeCustForUpdateComplete()
        {
            var mergeCusts = new List<InsertCustomerModel>();

            if (this.InsertCustomer != null && this.InsertCustomer.Any())
            {
                mergeCusts.AddRange(this.InsertCustomer);
            }

            if (this.CustInfoVVip != null && this.CustInfoVVip.Any())
            {
                var custs = this.CustInfoVVip.Select(x => new InsertCustomerModel
                {
                    CTSCustID = (int)x.CTSCustID,
                    CustID = x.CustID,
                    CustSubID = x.CustSubID,
                    UserName = x.UserName,
                    RegisterName = x.RegisterName,
                    SubscriberID = x.SubscriberID
                });

                mergeCusts.AddRange(custs);
            }

            return mergeCusts;
        }

    }
}
