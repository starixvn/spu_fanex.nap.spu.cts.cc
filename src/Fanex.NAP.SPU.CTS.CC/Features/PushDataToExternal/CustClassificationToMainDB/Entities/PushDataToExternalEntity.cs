namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    public class PushDataToExternalEntity
    {
        public bool IsPushCCToSoon88 { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }

        public int ExternalBatchSize { get; set; }

        public string ServiceName { get; set; }

        public bool IsWriteLog { get; set; }

        public string FeedSoon88API { get; set; }
    }
}