namespace Fanex.NAP.SPU.CTS.CC.Features.SystemParameter
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PreviousTimingExecutionGetDCCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ParamID")]
        public int ParamID { get; set; }

        public override string GetSettingKey()
               => "CTS_DC_SystemParameter_Get";

        public override bool IsValid()
            => true;
    }
}