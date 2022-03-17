namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.InactivateNewCustomer
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateStatusInactivateNewCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LastCustID")]
        public long LastCustID { get; set; }

        [SpParam(SqlParamName = "ip_LastScannedTime")]
        public DateTime LastScannedTime { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_InactiveNewMemberScan_UpdateStatus";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
