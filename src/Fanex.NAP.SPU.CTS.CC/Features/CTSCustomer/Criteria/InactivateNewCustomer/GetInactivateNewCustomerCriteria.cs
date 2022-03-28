namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class GetInactivateNewCustomerCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_InactiveNewMemberScan_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
