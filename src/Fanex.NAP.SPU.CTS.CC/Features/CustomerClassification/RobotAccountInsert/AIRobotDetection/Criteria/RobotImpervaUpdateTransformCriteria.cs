namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.AIRobotDetection.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RobotImpervaUpdateTransformCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LogIDs")]
        public string LogIDs { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_RobotImperva_Transform_Update";

        public override bool IsValid()
            => true;
    }
}
