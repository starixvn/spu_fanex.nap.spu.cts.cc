namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Models
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using System;

    public class ComplateLatencyCustomerModel
    {
        public ComplateLatencyCustomerModel() { }

        public ComplateLatencyCustomerModel(CustomerMainDBEntity customer)
        {
            CustID = customer.CustID;
            CustInfoRepUpdateTime = customer.CustInfoRepUpdateTime;
            DepCustSuperRepUpdateTime = customer.DepCustSuperRepUpdateTime;
            SiteRepUpdateTime = customer.SiteRepUpdateTime;
            CustProductStatusRepUpdateTime = customer.CustProductStatusRepUpdateTime;
        }

        public int CustID { get; set; }

        public DateTime? CustInfoRepUpdateTime { get; set; }

        public DateTime? DepCustSuperRepUpdateTime { get; set; }

        public DateTime? SiteRepUpdateTime { get; set; }

        public DateTime? CustProductStatusRepUpdateTime { get; set; }
    }
}
