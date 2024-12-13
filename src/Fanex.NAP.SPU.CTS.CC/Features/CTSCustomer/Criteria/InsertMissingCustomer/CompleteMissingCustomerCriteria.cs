namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompleteMissingCustomerCriteria : NonQueryCommand
    {

        [SpParam(SqlParamName = "ip_LastScannedID")]
        public long LastScannedID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Monitoring_MissingCustomer_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}