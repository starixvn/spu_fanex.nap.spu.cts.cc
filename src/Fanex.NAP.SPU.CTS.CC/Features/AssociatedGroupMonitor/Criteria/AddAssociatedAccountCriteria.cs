namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AddAssociatedAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_GroupID")]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustIDs")]
        public string CTSCustIDs { get; set; }

        [SpParam(SqlParamName = "ip_IsAuto")]
        public bool IsAuto { get; set; }

        [SpParam(SqlParamName = "ip_Remark")]
        public string Remark { get; set; }

        [SpParam(SqlParamName = "ip_CreatedBy")]
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Account_Add";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
