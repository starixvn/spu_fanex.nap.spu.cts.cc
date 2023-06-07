namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NormalClassificationRescanCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ListCustId")]
        public string ListCustId { get; set; }

        [SpParam(SqlParamName = "FunctionId")]
        public int FunctionId { get; set; }

        [SpParam(SqlParamName = "RescanTypeId")]
        public int RescanTypeId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_RealTime_Preprocess";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}