namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class NormalAccountBySportEntity
    {
        [XName("CustId")]
        public long CustID { get; set; }

        [XName("SportId")]
        public int SportID { get; set; }

        [XName("CategoryId")]
        public int CurrentCategoryID { get; set; }
    }
}
