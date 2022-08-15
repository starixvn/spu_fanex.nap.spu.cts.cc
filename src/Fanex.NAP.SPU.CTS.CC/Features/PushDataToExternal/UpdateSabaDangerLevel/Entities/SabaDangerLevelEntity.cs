namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    [XName("Root")]
    public class SabaDangerLevelEntity
    {
        [XName("custid")]
        public Int64 CustID { get; set; }

        [XName("SportType")]
        public int SportType { get; set; }

        [XName("action")]
        public int Action { get; set; }
    }
}