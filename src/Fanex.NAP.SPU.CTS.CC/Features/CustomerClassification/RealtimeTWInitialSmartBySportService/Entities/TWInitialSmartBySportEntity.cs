namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Entities
{
    using System;

    public class TWInitialSmartBySportEntity
    {
        public long CustID { get; set; }

        public long CTSCustID { get; set; }

        public int SportType { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public int CategoryID { get; set; }

        public int CategoryGroup { get; set; }

        public bool IsLicensee { get; set; }

        public DateTime DetectedDate { get; set; }

        public decimal Probability { get; set; }
    }
}
