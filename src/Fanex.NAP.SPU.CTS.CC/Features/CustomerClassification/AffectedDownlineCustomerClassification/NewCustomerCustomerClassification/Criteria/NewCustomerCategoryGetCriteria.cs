namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class NewCustomerCategoryGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "op_LastCTSCustID")]
        public UInt64 LastCTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_CheckCategoryInfoNewCust";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}