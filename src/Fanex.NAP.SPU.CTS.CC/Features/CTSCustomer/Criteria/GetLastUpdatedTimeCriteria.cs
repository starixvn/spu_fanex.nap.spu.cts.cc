namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class GetLastUpdatedTimeCriteria : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetLastUpdateTime";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}