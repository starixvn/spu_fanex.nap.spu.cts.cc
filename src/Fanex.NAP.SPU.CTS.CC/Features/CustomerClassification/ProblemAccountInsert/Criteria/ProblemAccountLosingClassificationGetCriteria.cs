namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountLosingClassificationGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustInfo")]
        public string CustJson { get; set; }

        [SpParam(SqlParamName = "FunctionId")]
        public int FunctionId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_ProblemAccountsClassification_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}