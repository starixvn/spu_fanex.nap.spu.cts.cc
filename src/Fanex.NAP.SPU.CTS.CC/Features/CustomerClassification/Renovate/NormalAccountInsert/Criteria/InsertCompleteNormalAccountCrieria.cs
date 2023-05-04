namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using Fanex.Data.Repository;

    public class InsertCompleteNormalAccountCrieria : NonQueryCommand
    {
        public string ListCustIds { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_NormalAccount_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
