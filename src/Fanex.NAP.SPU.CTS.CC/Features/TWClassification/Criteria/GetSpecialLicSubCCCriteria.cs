namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification.Criteria
{
    using Data.Repository;

    public class GetSpecialLicSubCCCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_SpecialLicSubCC_Response_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
