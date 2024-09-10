namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{    
    public class DetectUnauthorizedLoginCustEntity
    {
        public long CustID { get; set; }

        public int CTSCustID { get; set; }       

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public bool IsLicensee { get; set; }

        public int CategoryID { get; set; }    
    }
}
