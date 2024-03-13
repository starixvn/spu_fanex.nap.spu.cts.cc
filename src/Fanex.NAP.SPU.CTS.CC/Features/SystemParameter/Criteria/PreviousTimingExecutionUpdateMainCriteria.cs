namespace Fanex.NAP.SPU.CTS.CC.Features.SystemParameter.Criteria
{
    using Fanex.Data.Repository;

    public class PreviousTimingExecutionUpdateMainCriteria : NonQueryCommand
    {
        public int ParamId { get; set; }

        public string ParamValue { get; set; }

        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_SystemParameter_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
