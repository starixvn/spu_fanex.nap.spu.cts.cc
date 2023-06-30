namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria
{
    using Fanex.Data.Repository;

    public class ClassifyNormalPoolCustomerBySportCriteria : NonQueryCommand
    {
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_NormalPool_Classify";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
