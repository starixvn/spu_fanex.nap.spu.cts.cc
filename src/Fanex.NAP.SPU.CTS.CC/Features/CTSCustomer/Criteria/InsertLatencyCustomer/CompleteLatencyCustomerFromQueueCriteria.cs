namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.InsertLatencyCustomer
{
    using Fanex.Data.Repository;

    public class CompleteLatencyCustomerFromQueueCriteria : NonQueryCommand
    {
        public string CustIds { get; set; }

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
