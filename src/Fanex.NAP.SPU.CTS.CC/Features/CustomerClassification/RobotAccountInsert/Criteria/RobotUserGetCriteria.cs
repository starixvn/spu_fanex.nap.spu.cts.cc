namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RobotUserGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "LastTstamp")]
        public long? LastTimestamp { get; set; }

        [SpParam(SqlParamName = "MaxTstamp")]
        public long MaxTimestamp { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_RobotListSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}