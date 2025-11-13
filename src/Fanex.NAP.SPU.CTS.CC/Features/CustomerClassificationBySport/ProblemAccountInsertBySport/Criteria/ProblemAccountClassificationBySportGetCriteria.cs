namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountClassificationBySportGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ListCustInfo")]
        public string CustJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_ProblemAccountsClassificationBySport_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
