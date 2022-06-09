namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.DataLogTracking
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InputErrorLogInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LogInfo")]
        public string LogInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Archive_Log_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}