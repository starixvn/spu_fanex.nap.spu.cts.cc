namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.DailyClassification
{
    using System;
    using System.Xml.Serialization;

    public class CustClassificationDailyEntity
    {
        [XmlIgnore]
        public int BatchID { get; set; }

        public long CustId { get; set; }

        public int SportGroupId { get; set; }

        public int CategoryId { get; set; }

        [XmlIgnore]
        public DateTime CreatedTime { get; set; }
    }
}