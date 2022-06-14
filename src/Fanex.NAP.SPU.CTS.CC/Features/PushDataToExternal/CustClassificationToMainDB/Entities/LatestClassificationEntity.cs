namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class LatestClassificationEntity
    {
        [XName("id")]
        public long CustId { get; set; }

        [XName("c")]
        public int CustomerClass { get; set; }

        [XName("t")]
        public int Category { get; set; }
    }
}