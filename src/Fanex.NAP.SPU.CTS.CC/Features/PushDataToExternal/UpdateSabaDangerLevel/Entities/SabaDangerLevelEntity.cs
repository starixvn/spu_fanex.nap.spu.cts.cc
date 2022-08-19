namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;

    [XName("r")]
    public class SabaDangerLevelEntity
    {
        [XName("CustID")]
        public UInt64 CustID { get; set; }

        [XName("SportType")]
        public int SportType { get; set; }

        /// <summary>
        /// -1: remove, <>-1: insert
        /// </summary>
        [XName("Action")]
        public int Action { get; set; }
    }
}