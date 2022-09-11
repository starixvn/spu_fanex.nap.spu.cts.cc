namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ScanDailyNormalAccountsClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustIds")]
        public string ListCustIDs { get; set; }
      
        [SpParam(SqlParamName = "FunctionId")]
        public int FunctionID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_BySport_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
