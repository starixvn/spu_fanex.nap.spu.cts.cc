namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class GetLatencyCustomerFromQueueCriteria : CriteriaBase
    {
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustomerLatency_Queue_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
