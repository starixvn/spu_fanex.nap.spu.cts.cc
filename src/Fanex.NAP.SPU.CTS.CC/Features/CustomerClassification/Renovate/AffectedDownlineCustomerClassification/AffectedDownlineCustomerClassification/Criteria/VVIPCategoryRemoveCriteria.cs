namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.AffectedDownline
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class VVIPCategoryRemoveCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromAction")]
        public int FromAction { get; set; }

        [SpParam(SqlParamName = "ip_UplineRoleID")]
        public int? UplineRoleID { get; set; }

        [SpParam(SqlParamName = "ip_QueueID")]
        public long? QueueID { get; set; }

        [SpParam(SqlParamName = "ip_AffectDownline")]
        public bool AffectDownline { get; set; }

        [SpParam(SqlParamName = "ip_CustJson")]
        public string CustJson { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_VVIP_Remove";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}