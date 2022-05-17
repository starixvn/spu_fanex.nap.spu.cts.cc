namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class BetLimitByEvidenceAddingUpdateCriteria : NonQueryCommand
    {
        public string CustIDList { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
            => "NAP_BLControl_EvidenceFlagged";

        public override bool IsValid()
            => true;
    }
}