namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;

    public class GetCustHasChangeForScanningCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_BySport_RealTime_GetChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}