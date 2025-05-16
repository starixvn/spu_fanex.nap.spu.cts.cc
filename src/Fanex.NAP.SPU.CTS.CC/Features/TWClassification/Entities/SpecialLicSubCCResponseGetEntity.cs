namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using System;

    public class SpecialLicSubCCResponseGetEntity
    {
        public int ID { get; set; }

        public long CustID { get; set; }

        public int APICustomerClass { get; set; }

        public int LatestCustomerClass { get; set; }

        public int StatusCode { get; set; }

        public string ItemNameDisplay { get; set; }

        public DateTime? ResponseTime { get; set; }

        public int? ResponseHttpCode { get; set; }
    }
}
