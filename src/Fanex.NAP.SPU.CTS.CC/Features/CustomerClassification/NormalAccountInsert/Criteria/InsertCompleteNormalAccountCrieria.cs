namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertCompleteNormalAccountCrieria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        [SpParam(SqlParamName = "ListCustId")]
        public string ListCustId { get; set; }

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
