namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class LogInputDataCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputData")]
        public string InputData { get; set; }

        [SpParam(SqlParamName = "ip_FunctionID")]
        public int FunctionID { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_LogInputData";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
