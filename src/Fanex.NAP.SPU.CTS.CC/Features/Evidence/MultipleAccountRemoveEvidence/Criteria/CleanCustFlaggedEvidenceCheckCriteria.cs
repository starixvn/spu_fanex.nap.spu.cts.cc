namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CleanCustFlaggedEvidenceCheckCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CTSCustIDList")]
        public string CTSCustIds { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustEvidence_CheckCleanCustFlagged";

        public override bool IsValid()
            => true;
    }
}