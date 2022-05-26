namespace Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification
{    
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class LatestClassificationModel
    {
        [XName("id")]
        public int CustId { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

        [XName("t")]
        public int Category { get; set; }

        [XmlIgnore]
        public DateTime ScannedTime { get; set; }
    }
}