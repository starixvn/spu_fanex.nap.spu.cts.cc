namespace Fanex.NAP.SPU.CTS.Features.UserPermission.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertUserPermissionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserIDList")]
        public string UserIDList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Common_InsertUserPermission";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(UserIDList);
        }
    }
}
