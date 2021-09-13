namespace Fanex.NAP.SPU.CTS.CC.Features.Evidence.MultipleAccountRemoveEvidence
{
    using Fanex.Data.Repository;

    public class CustEvidenceQueueGetCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustEvidenceQueue_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}