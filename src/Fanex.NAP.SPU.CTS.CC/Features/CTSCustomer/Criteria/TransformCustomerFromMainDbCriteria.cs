namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TransformCustomerFromMainDbCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustomerList")]
        public string CustomerJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CTSCustomer_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerJson);
        }
    }
}