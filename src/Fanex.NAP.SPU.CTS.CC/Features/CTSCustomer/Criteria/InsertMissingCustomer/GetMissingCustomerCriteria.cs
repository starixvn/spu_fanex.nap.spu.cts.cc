namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class GetMissingCustomerCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_MissingCustomer_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}