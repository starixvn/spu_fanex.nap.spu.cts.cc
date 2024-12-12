namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ScanAssociationByDeviceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "op_LastCTSAssDevID")]
        public long LastCTSAssDevID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DgrAssociation_ScanDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
