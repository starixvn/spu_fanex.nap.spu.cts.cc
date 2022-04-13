namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.InactivateNewCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class UpdateStatusInactivateNewCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LastCTSCustID")]
        public long LastCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_LastCreatedDate")]
        public DateTime LastCreatedDate { get; set; }

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