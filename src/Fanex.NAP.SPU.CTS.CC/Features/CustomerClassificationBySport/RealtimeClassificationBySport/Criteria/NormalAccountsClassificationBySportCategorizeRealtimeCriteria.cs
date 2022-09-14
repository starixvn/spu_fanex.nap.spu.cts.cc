namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;

    public class NormalAccountsClassificationBySportCategorizeRealtimeCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_NormalAccountsClassification_BySport_RealTime";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomersXML);
        }
    }
}