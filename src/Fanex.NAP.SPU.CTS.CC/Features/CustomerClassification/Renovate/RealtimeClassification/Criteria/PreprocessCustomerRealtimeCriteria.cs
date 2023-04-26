namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class PreprocessCustomerRealtimeCriteria : NonQueryCommand
    {     
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_Realtime_Preprocess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
