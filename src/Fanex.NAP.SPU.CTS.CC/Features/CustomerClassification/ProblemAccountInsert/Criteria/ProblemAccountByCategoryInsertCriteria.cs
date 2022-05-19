namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountByCategoryInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfoJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_ProblemAccount_InsertByCategory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}