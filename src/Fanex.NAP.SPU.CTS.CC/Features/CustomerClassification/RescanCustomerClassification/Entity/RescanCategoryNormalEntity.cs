namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class RescanCategoryNormalEntity
    {
        [XName("CustId")]
        public long CustID { get; set; }

        [XName("CategoryId")]
        public int CategoryID { get; set; }

    }
}
