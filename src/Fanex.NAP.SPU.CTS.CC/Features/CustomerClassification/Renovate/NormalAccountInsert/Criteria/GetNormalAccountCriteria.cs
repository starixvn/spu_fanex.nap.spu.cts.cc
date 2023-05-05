namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNormalAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_NormalAccount_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
