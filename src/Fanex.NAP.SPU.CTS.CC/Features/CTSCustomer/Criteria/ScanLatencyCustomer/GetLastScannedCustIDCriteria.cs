namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.ScanLatencyCustomer
{
    using Fanex.Data.Repository;

    public class GetLastScannedCustIDCriteria : CriteriaBase
    {
        public int ScannedTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustomerLatency_GetLastScannedCustID";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
