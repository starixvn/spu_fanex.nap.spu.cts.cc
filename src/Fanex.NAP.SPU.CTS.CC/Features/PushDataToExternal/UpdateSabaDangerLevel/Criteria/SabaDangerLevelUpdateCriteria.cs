namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SabaDangerLevelUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustList")]
        public string CustList { get; set; }

        [SpParam(SqlParamName = "ErrCode")]
        public int ErrCode { get; set; }

        [SpParam(SqlParamName = "ErrMsg")]
        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUdpDanger_JobExclusion";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}