namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ScanSystemFormulaBetCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LastCustID")]
        public uint LastCustId { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_TWProblemDetectionMonitor_Scan_Completed";

        public override bool IsValid()
            => true;
    }
}
