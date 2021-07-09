namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;

    public class NewClassificationGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GetNewClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}