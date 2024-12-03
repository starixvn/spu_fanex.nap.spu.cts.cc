namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PreprocessDangerousAssociationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustJson")]
        public string CustJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_DangerousAssociation_Preprocess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
