namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ClassifyNormalPoolCustomerCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "CustomersXML")]
        public string CustomersXML { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_NormalPool_Classify";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
