namespace Fanex.NAP.SPU.CTS.Features.UpdateLastLogin.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TransformUpdatingLastLoginTimeCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Batch")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_UpdateLastLoginTime";
        }

        public override bool IsValid()
        {
            return BatchSize > 0;
        }
    }
}
