namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Criteria
{
    using Fanex.Data.Repository;

    public class CheckDangerousAssociationPerformanceCriteria : CriteriaBase
    {
        public string ListCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_DgrAssociation_PerformanceCheck";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
