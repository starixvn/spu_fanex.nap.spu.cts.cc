namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetInactivateNewCustomerCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastCategoryID")]
        public int LastCategoryID { get; set; }

        [SpParam(SqlParamName = "op_LastCustID")]
        public UInt64 LastCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DailyScan_GetInactiveNewMember";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}