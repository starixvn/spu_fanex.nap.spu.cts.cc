namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Criteria
{
    using Fanex.Data.Repository;

    public class ScanLatencyCustomerCriteria : CriteriaBase
    {

        public int FromCustID { get; set; }

        public int ToCustID { get; set; }

        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_CustomerLatency_Scan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
