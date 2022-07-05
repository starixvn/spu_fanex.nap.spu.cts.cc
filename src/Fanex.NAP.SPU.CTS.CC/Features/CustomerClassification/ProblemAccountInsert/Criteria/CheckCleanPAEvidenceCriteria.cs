namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckCleanPAEvidenceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDList")]
        public string CTSCustIds { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_CheckCleanCust";

        public override bool IsValid()
            => true;
    }
}