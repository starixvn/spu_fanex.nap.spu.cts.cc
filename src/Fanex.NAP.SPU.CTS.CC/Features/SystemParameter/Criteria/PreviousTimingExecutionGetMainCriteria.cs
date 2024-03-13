namespace Fanex.NAP.SPU.CTS.CC.Features.SystemParameter.Criteria
{
    using Fanex.Data.Repository;

    public class PreviousTimingExecutionGetMainCriteria : CriteriaBase
    {
        public int ParamID { get; set; }

        public override string GetSettingKey()
               => "CTS_SystemParameter_Get";

        public override bool IsValid()
            => true;
    }
}
