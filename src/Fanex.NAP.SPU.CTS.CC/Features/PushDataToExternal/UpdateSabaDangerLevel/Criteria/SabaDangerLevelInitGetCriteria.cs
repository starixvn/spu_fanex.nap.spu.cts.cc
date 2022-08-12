namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class SabaDangerLevelInitGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastCategoryID")]
        public int LastCategoryID { get; set; }        
        
        [SpParam(SqlParamName = "op_LastCustID")]
        public Int64 LastCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_SabaDangerLevel_Init";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}