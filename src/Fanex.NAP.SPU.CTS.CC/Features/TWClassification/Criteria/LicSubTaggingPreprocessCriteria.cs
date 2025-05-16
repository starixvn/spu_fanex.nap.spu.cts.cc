namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Fanex.Data.Repository;

    public class LicSubTaggingPreprocessCriteria : NonQueryCommand
    {        
        public string ListCustId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_LicSubTagging_Preprocess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }   
}
