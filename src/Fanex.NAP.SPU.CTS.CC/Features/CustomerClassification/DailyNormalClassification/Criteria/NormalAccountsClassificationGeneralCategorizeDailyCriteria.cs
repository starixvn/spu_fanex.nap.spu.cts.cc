namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification
{
    using Fanex.Data.Repository;

    public class NormalAccountsClassificationGeneralCategorizeDailyCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }      

        public int FunctionId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_GeneralCategorize_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}