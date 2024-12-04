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
            throw new System.NotImplementedException();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
