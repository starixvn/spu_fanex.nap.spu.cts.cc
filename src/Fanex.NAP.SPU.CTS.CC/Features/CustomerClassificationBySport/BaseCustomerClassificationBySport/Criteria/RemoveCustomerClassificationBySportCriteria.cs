namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveCustomerClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputFlowID")]
        public int InputFlowID { get; set; }

        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustClassificationJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}