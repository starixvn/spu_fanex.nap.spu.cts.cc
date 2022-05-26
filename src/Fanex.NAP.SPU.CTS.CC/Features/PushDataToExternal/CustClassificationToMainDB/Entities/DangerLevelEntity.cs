namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Xml.Serialization;

    [XName("r")]
    public class DangerLevelEntity
    {
        [XName("id")]
        public long CustId { get; set; }

        [XName("l")]
        public int DangerLevel { get; set; }
    }
}