namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Entities
{
    public class InsertCustomerModel
    {
        public int CTSCustID { get; set; }

        public long CustID { get; set; }

        public int CustSubID { get; set; }

        public bool IsExisting { get; set; }

        public int SubscriberID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; }
    }
}