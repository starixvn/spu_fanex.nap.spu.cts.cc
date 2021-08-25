namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateUsername2Criteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Customer")]
        public string CustomerJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_UpdateUserName2";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerJson);
        }
    }
}