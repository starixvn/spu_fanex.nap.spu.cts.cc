namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountProbationInsertCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ProblemAccount")]
        public string ProblemAccountJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_ProblemAccountProbation_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}