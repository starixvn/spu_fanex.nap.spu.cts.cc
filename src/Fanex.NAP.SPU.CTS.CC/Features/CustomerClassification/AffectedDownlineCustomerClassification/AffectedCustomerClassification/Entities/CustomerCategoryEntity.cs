namespace Fanex.NAP.SPU.CTS.CC.Features.DownlineCustomerClassification
{
    public class CustomerCategoryEntity
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public int CreatedBy { get; set; }

        public string Remark { get; set; }        

        public long LastDownlineCTSCustID { get; set; }

    }
}
