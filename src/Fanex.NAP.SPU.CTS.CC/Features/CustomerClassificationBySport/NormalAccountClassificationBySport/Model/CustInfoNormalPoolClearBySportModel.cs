namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class CustInfoNormalPoolClearBySportModel
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportGroup")]
        public int SportGroup { get; set; }

    }
}
