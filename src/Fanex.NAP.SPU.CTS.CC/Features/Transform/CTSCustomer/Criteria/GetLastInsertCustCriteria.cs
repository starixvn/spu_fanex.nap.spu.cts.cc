namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class GetLastInsertCustCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_GetLastInsertCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
