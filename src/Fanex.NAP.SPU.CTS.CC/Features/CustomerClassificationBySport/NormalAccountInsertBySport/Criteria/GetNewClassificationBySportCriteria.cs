namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.Data.Repository;

    public class GetNewClassificationBySportCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_BySport_GetNewClassification";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
