namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class DangerLevelEntity
    {
        [XName("id")]
        public long CustId { get; set; }

        [XName("l")]
        public int DangerLevel { get; set; }
    }
}