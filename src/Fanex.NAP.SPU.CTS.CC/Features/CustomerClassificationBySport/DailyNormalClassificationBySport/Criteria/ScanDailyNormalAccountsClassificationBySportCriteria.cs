namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ScanDailyNormalAccountsClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustomersXML")]
        public string CustomersXML { get; set; }
            
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
