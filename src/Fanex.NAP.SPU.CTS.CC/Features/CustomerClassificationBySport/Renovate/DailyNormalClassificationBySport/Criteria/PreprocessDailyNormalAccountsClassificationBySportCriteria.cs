namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PreprocessDailyNormalAccountsClassificationBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustomersXML")]
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_Daily_Preprocess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
