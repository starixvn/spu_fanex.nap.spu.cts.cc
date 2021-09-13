namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Xml.Serialization;

    [XName("ProbationModel")]
    public class ProbationEntity
    {
        [XName("CustId")]
        public long CustId { get; set; }

        [XmlIgnore]
        public int SportGroupId { get; set; }

        // 0. VIP & Normal OR 1.Update to Smart, High Risk, VIP & Normal
        [XName("IsProbationLastDay")]
        public int IsProbationLastDay { get; set; }

        [XmlIgnore]
        public int CategoryId { get; set; }

        [XmlIgnore]
        public bool IsPassed { get; set; }
    }
}