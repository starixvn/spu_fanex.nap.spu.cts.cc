namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class NormalAccountBySportEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportGroup")]
        public int SportGroup { get; set; }
       
    }
}
