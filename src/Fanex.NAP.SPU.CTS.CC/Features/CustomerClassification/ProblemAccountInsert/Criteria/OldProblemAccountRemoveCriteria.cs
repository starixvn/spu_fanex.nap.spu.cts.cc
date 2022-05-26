namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class OldProblemAccountRemoveCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_PA_ClearOld";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}