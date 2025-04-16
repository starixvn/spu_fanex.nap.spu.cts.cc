namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class GetLastInsertCustCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetLastInsertCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}