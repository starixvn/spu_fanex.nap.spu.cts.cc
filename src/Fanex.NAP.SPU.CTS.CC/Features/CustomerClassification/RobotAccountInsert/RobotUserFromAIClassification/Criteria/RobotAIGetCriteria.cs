namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class RobotAIGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastID")]
        public UInt64 LastID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_RobotClassification_Scan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}