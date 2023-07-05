namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class CustClassificationRealtimeBySportEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportId")]
        public int SportId { get; set; }

        [XName("CategoryId")]
        public int CategoryId { get; set; }
    }
}
