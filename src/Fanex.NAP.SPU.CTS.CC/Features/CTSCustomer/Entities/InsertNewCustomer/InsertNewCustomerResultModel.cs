namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities
{
    public class InsertNewCustomerResultModel
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int CustSubID { get; set; }

        public int SubscriberID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; }

        public int RoleID { get; set; }
    }
}