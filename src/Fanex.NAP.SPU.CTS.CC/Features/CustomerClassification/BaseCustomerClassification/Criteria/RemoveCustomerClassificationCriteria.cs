namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveCustomerClassificationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_InputFlowID")]
        public int InputFlowID { get; set; }

        [SpParam(SqlParamName = "ip_FromAction")]
        public int FromAction { get; set; }

        [SpParam(SqlParamName = "ip_AffectDownline")]
        public bool AffectDownline { get; set; }

        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustClassificationJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
