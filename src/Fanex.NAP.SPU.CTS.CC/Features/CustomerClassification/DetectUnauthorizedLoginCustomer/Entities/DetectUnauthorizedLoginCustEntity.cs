namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DetectUnauthorizedLoginCustomer
{
    using System;
    
    public class DetectUnauthorizedLoginCustEntity
    {
        public long CustID { get; set; }

        public int CTSCustID { get; set; }

        public DateTime FirstBetDate { get; set; }

        public DateTime DetectionDate { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public bool IsLicensee { get; set; }

        public int CategoryID { get; set; }

        public int Bot { get; set; }

        public int InvalidBrowser { get; set; }

        public double BotPCT { get; set; }

        public double InvalidBrowserPCT { get; set; }
    }
}
