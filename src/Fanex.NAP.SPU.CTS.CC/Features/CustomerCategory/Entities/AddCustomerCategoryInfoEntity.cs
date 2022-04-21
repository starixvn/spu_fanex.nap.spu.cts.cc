namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory
{  
    public class AddCustomerCategoryInfoEntity
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int SubscriberID { get; set; }        

        public long CreatedBy { get; set; }

        public string Remark { get; set; }
    }
}
