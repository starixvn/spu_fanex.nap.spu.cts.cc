namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCTSCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NewCTSCustomerCategoryGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "op_LastCTSCustID")]
        public long LastCTSCustID { get; set; }

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