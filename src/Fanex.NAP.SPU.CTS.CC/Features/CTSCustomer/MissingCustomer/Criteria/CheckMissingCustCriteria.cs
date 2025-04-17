namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckMissingCustCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerListJson { get; set; }

        [SpParam(SqlParamName = "ip_LastCTSCustID")]
        public ulong? LastCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_QueryTypeID")]
        public int QueryTypeID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustomerLatency_CheckMissingCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
