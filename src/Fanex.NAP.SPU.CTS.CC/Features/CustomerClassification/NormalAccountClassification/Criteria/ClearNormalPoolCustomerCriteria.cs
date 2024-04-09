namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using Fanex.Data.Repository;

    public class ClearNormalPoolCustomerCriteria : NonQueryCommand
    {
        public string ListCustId { get; set; }

        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_NormalPool_Clear";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}