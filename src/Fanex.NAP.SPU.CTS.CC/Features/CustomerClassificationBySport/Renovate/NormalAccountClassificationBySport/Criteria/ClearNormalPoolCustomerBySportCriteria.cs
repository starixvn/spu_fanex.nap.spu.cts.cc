namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria
{
    using Fanex.Data.Repository;

    public class ClearNormalPoolCustomerBySportCriteria : NonQueryCommand
    {      
        public string CustomersXML { get; set; }

        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_NormalPool_Clear";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
