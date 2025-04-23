namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompleteLatencyCustomerFromQueueCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Customer")]
        public string CustomerJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustomerLatency_Queue_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
