namespace Fanex.NAP.SPU.CTS.Features.AffectedEvidence.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateBetLimitByEvidenceRemovingCriteria : NonQueryCommand
    {
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_BLControl_EvidenceFlagged";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustIDList);
        }
    }
}
