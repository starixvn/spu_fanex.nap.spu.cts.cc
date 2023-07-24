namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport
{
    using Fanex.Data.Repository;

    public class PreprocessCustomerRealtimeBySportCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_Realtime_Preprocess";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomersXML);
        }
    }
}
