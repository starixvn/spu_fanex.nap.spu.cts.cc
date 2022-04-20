namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory
{
    public class CustomerCategoryEntity
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int SubscriberID { get; set; }

        public int CreatedBy { get; set; }

        public string Remark { get; set; }

        public int CategoryID { get; set; }

        public long LastDownlineCTSCustID { get; set; }

    }
}
