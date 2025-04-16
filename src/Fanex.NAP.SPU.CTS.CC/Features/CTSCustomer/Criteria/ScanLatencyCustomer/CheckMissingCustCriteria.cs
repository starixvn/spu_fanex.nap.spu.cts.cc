namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.ScanLatencyCustomer
{
    using Fanex.Data.Repository;

    public class CheckMissingCustCriteria : CriteriaBase
    {
        public string CustIDs { get; set; }
        public override string GetSettingKey()
        {
            return "CTS_DC_CustomerLatency_CheckMissingCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
