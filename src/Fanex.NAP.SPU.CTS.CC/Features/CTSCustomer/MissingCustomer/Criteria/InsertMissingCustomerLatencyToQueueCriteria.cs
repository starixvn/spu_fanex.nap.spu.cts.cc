namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class InsertMissingCustomerLatencyToQueueCriteria : NonQueryCommand
    {
        public string CustIDs { get; set; }

        public int DataSource { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustomerLatency_Queue_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
