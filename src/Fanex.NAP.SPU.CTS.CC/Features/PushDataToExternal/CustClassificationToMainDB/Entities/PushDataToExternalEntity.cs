namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    public class PushDataToExternalEntity
    {
        public PushDataToExternalEntity()
        {
            IsWriteLog = false;
            IsUpdateDangerToMainDB = true;
        }

        public bool IsUpdateDangerToMainDB { get; set; }

        public int ExternalBatchSize { get; set; }

        public string ServiceName { get; set; }

        public bool IsWriteLog { get; set; }

        //AFC
        public bool IsSyncAFCData { get; set; }

        public string AFCInsertAccountURL { get; set; }

        public string AFCDeleteAccountURL { get; set; }

        public RoleGroupType RoleGroup { get; set; } // default sp is member
    }
}