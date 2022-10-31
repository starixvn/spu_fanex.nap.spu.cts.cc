namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using Fanex.NAP.SPU.CTS.Shares;

    [XName("r")]
    public class DangerLevelModel
    {
        [XName("id")]
        public long CustId { get; set; }

        [XName("l")]
        public int DangerLevel { get; set; }
    }
}
