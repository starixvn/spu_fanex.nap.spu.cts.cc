namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Criteria
{
    using Fanex.Data.Repository;
    using Fanex.Data;

    public class NewCTSCustomerInsertCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerJson { get; set; }

        [SpParam(SqlParamName = "ip_IsNewCustomer")]
        public bool IsNewCustomer { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerJson);
        }
    }
}
