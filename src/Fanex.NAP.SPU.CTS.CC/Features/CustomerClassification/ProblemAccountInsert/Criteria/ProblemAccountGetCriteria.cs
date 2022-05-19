namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ProblemAccountGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "CustIds")]
        public string CustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_ProblemAccountManagement_GetByCustIDs";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}