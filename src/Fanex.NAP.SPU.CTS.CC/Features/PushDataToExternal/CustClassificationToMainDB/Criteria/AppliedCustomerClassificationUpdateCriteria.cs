namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AppliedCustomerClassificationUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ProcessedCustList")]
        public string ProblemCustomerJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_UpdateIsAppliedCC";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}