namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification
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

        [XName("IsCheckBetCount")]
        public bool IsCheckBetCount { get; set; }

        [XName("IsProbationLastDay")]
        public bool IsProbationLastDay { get; set; }
    }

}
