namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class ProbationScanClassificationBySportEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportGroup")]
        public int SportGroup { get; set; }       
    }
}
