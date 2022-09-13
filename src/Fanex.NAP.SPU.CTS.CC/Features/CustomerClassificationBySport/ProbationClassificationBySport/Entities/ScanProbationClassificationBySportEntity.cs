namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class ScanProbationClassificationBySportEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportId")]
        public int SportId { get; set; }      
    }
}
