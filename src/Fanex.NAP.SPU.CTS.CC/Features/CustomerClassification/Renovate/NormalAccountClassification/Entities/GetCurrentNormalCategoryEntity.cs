namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification
{
    public class GetCurrentNormalCategoryEntity
    {
        public long CustID { get; set; }

        public int CategoryID { get; set; }

        public bool IsNewCreated { get; set; }

        public bool IsProbationLastDay { get; set; }
    }
}