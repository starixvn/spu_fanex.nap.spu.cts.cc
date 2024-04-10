namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDailyNormalAccountsClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "op_LastCategoryID")]
        public int LastCategoryID { get; set; }

        [SpParam(SqlParamName = "op_LastCustID")]
        public ulong LastCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_DailyScan_GetNormal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
