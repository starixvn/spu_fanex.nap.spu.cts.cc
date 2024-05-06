namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification
{
    using Fanex.Data.Repository;

    public class DailyClassifyCriteria : NonQueryCommand
    {
        public string ListCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_Daily_Preprocess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}