namespace Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;  

    public class TWRobotImpervaGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "LastLogID")]
        public long LastLogID { get; set; }

        [SpParam(SqlParamName = "MaxLogID")]
        public long MaxLogID { get; set; }      

        public override string GetSettingKey()
        {
            return "CTS_RobotImperva_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}