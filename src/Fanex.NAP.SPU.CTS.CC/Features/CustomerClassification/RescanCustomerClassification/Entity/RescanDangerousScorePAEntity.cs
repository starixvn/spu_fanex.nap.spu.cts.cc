namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using System;

    public class RescanDangerousScorePAEntity
    {
        public long CustID { get; set; }

        public int CTSCustID { get; set; }

        public int SubscriberID { get; set; }

        public int RoleID { get; set; }

        public int CategoryID { get; set; }

        public bool IsLicensee { get; set; }

        public bool IsFromAI { get; set; }

        public int CategoryGroup { get; set; }

        public DateTime ClassifiedDate { get; set; }

        public decimal ClassifiedScore { get; set; }

    }
}
