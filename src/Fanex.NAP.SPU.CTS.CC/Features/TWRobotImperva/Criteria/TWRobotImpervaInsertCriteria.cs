namespace Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TWRobotImpervaInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RobotInfo")]
        public string RobotInfo { get; set; }
      
        public override string GetSettingKey()
        {
            return "CTS_DC_RobotImperva_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}