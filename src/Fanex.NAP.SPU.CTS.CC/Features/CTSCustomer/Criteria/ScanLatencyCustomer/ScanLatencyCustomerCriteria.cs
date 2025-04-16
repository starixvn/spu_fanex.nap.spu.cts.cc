namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.ScanLatencyCustomer
{
    using Fanex.Data.Repository;

    public class ScanLatencyCustomerCriteria : CriteriaBase
    {

        public int LastCustID { get; set; }

        public int NextCustID { get; set; }

        public int NextScannedCustID { get; set; }

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
