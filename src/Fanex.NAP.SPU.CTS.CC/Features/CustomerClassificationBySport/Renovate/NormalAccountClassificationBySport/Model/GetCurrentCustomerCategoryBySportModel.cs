namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class GetCurrentCustomerCategoryBySportModel
    {
        [XName("CustId")]
        public long CustID { get; set; }

        [XName("SportGroup")]
        public int SportGroup { get; set; }

        [XName("CategoryId")]
        public int CategoryID { get; set; }      

        [XName("IsCheckBetCount")]
        public bool IsCheckBetCount { get; set; }

        [XName("IsProbationLastDay")]
        public bool IsProbationLastDay { get; set; }
    }
}
