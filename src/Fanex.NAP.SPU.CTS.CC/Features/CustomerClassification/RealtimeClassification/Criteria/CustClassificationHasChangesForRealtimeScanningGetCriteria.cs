namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;

    public class CustClassificationHasChangesForRealtimeScanningGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GeneralCategorize_RealTime_GetChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}