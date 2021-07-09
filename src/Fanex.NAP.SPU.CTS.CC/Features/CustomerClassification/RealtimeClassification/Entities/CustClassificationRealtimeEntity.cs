namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    [XName("Root")]
    public class CustClassificationRealtimeEntity
    {
        [XmlIgnore, JsonIgnore]
        public int BatchID { get; set; }

        [XName("CustId")]
        public long CustId { get; set; }

        [XName("SportGroupId")]
        public int SportGroupId { get; set; }

        [XName("CategoryId"), JsonIgnore]
        public int CategoryId { get; set; }

        [XmlIgnore, JsonIgnore]
        public DateTime CreatedTime { get; set; }
    }
}