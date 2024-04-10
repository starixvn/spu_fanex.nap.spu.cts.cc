namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class CustSportXMLModel
    {
        [XName("CustId")]
        public long CustID { get; set; }

        [XName("SportGroup")]
        public int SportGroup { get; set; }
    }
}
