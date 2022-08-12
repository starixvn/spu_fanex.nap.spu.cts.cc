namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SabaDangerLevelScanCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastHistoryID")]
        public int LastHistoryID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_SabaDangerLevel_Scan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}