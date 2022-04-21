namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RemoveCustomerCategoryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromAction")]
        public int FromAction { get; set; }

        [SpParam(SqlParamName = "ip_UplineRoleID")]
        public int UplineRoleID { get; set; }

        [SpParam(SqlParamName = "ip_AffectDownline")]
        public bool AffectDownline { get; set; }

        [SpParam(SqlParamName = "ip_CustJson")]
        public string CustJson { get; set; }

        [SpParam(SqlParamName = "op_ErrorMessage")]
        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_RemoveVVIP";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
