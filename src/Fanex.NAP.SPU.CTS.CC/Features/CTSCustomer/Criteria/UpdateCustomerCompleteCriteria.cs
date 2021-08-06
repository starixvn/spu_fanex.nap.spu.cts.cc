namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class UpdateCustomerCompleteCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerInfo")]
        public string CustomerInfo { get; set; }

        [SpParam(SqlParamName = "ip_LastUpdateTime")]
        public DateTime? LastUpdateTime { get; set; }

        [SpParam(SqlParamName = "ip_LastUpdateCustID")]
        public Int64? LastUpdateCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_UpdateCustomerComplete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}