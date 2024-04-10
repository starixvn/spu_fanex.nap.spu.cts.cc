namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CompleteNormalAccountBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        [SpParam(SqlParamName = "CustomersXML")]
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_NormalAccount_Complete";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
