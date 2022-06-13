namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    public class LatestCCDangerLevelEntity
    {
        public long CustID { get; set; }

        public int CategoryID { get; set; }

        public int CustomerClass { get; set; }

        public int? DangerLevel { get; set; }

        public int? DangerLevelType { get; set; }
    }
}