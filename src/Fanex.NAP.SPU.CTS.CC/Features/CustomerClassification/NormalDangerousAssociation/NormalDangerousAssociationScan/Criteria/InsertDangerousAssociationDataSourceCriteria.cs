namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertDangerousAssociationDataSourceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustIDList")]
        public string CustIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DgrAssociation_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
