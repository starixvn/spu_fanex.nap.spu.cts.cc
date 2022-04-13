namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountLosingClassificationGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustIds")]
        public string ListCustIds { get; set; }

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