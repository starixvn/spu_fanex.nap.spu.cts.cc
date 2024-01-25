namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.Accounts.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class ReactivedCustomerLogInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LastScanTime")]
        public DateTime? LastScanTime { get; set; }

        [SpParam(SqlParamName = "ip_RequestTime")]
        public DateTime? RequestTime { get; set; }

        [SpParam(SqlParamName = "ip_MaxLogTime")]
        public DateTime? MaxLogTime { get; set; }

        [SpParam(SqlParamName = "ip_CustInfoTime")]
        public DateTime? CustInfoTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_ArchiveCustomer_Reactive_InsertLog";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}