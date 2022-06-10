namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    public class PushDataToExternalEntity
    {
        public PushDataToExternalEntity()
        {
            IsWriteLog = false;
            IsPushCCToSoon88 = true;
            IsUpdateDangerToMainDB = true;
        }

        public bool IsPushCCToSoon88 { get; set; }

        public string FeedSoon88API { get; set; }

        public bool IsUpdateDangerToMainDB { get; set; }

        public int ExternalBatchSize { get; set; }

        public string ServiceName { get; set; }

        public bool IsWriteLog { get; set; }

        //AFC
        public bool IsSyncAFCData { get; set; }

        public string AFCInsertAccountURL { get; set; }

        public string AFCDeleteAccountURL { get; set; }
    }
}