namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class GetCurrentCustomerCategoryModel
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("CategoryId")]
        public int CategoryId { get; set; }

        [XName("IsNewCreated")]
        public bool IsNewCreated { get; set; }

        [XName("IsRealtimeOnly")]
        public bool IsRealtimeOnly { get; set; }

        [XName("IsProbationLastDay")]
        public bool IsProbationLastDay { get; set; }

        [XName("ScanTaggingType")]
        public int ScanTaggingType { get; set; }

        [XName("ScanSpecialLicSubType")]
        public int ScanSpecialLicSubType { get; set; }

        [XName("IsSpecialLicSubCC")]
        public bool IsSpecialLicSubCC { get; set; }
    }
}
