namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using System;

    public class CustInfoClassificationEntity
    {
        public long CTSCustID { get; set; }

        public long CustID { get; set; }

        public int CustomerClass { get; set; }

        public int SportID { get; set; }

        public int CategoryID { get; set; }

        public DateTime ScannedTime { get; set; }

        public bool IsPA { get; set; }

        public bool IsScanDgrAssociation { get; set; }
    }
}
