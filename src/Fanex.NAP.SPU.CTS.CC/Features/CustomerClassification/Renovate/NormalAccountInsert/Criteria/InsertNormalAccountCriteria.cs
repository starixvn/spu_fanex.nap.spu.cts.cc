namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertNormalAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NewClassification")]
        public string NewClassification { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_InsertNormalAccount";
        }

        public override bool IsValid()
        {
            return true;
        }

    }
}
