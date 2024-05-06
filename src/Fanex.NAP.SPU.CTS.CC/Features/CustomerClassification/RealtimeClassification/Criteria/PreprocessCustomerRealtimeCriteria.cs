namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{  
    using Fanex.Data.Repository;

    public class PreprocessCustomerRealtimeCriteria : NonQueryCommand
    {     
        public string ListCustId { get; set; }

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
