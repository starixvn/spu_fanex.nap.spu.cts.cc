namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PreprocessDangerousAssociationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfoJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DgrAssociation_DevicePool_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
