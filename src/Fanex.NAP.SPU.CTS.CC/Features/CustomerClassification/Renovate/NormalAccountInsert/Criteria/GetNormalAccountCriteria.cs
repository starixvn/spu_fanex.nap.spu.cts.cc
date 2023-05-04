namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using Fanex.Data.Repository;

    public class GetNormalAccountCriteria : CriteriaBase
    {
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
