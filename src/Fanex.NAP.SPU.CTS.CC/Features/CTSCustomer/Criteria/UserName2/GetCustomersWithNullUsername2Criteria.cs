namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCustomersWithNullUsername2Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_PreCTSCustID")]
        public UInt64 PreCTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "ip_IsCustSub")]
        public bool IsCustSub { get; set; }

        [SpParam(SqlParamName = "op_LastCTSCustID")]
        public UInt64 LastCTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetNullUserName2";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}