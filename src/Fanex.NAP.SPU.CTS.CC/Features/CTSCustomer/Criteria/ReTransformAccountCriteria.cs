namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.Accounts.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ReTransformAccountCriteria : NonQueryCommand
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
            return !string.IsNullOrEmpty(CustomerInfo) && LastInsertCustID is not null;
        }
    }
}