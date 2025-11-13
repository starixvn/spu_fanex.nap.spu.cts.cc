namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountProbationBySportInsertCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_ProblemAccount")]
        public string ProblemAccountJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassificationBySport_DailyScanPA_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
