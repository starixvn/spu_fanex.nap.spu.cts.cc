namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class SabaDangerLevelScanCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastHistoryID")]
        public UInt64 LastHistoryID { get; set; }

        [SpParam(SqlParamName = "op_Agency_LastHistoryID")]
        public UInt64 AgencyLastHistoryID { get; set; }

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