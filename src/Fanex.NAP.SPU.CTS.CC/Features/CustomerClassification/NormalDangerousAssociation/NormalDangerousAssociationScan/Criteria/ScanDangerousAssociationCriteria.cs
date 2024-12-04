namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ScanDangerousAssociationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "op_LastScannedID")]
        public long LastScannedID { get; set; }

        public override string GetSettingKey()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
