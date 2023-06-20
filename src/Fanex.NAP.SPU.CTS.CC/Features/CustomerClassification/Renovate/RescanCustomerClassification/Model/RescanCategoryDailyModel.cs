namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class RescanCategoryDailyModel
    {
        [XName("CustId")]
        public long CustID { get; set; }

        [XName("CategoryId")]
        public int CurrentCategoryID { get; set; }
    }
}
