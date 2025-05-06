namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class UpdateCompleteAndRetranCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Customer")]
        public string CustomerInfo { get; set; }

        [SpParam(SqlParamName = "ip_LastInsertCustID")]
        public long? LastInsertCustID { get; set; }

        [SpParam(SqlParamName = "ip_IsInsertMissing")]
        public bool IsInsertMissingCust { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_InsertComplete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
