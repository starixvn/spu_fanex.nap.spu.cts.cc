namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountInsertBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertNormalAccountBySportCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_NewClassification")]
        public string NewClassification { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_BySport_InsertNormalAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
