namespace Fanex.NAP.SPU.CTS.Features.AffectedEvidence.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TransformAssociationAffectedEvidenceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Batch")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidence_InsertFromAssociation";
        }

        public override bool IsValid()
        {
            return BatchSize > 0;
        }
    }
}
