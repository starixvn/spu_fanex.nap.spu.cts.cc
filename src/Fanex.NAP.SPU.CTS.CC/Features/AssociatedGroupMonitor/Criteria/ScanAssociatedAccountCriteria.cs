namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ScanAssociatedAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "op_GroupID")]
        public long GroupID { get; set; }

        [SpParam(SqlParamName = "op_GroupName")]
        public string GroupName { get; set; }

        [SpParam(SqlParamName = "op_ABI")]
        public int ABI { get; set; }

        [SpParam(SqlParamName = "op_PACategoryID")]
        public int PACategoryID { get; set; }

        [SpParam(SqlParamName = "op_LastCTSCustID")]
        public long LastCTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_AssociatedGroup_Scan";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
