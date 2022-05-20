namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class RobotAccountInsertCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_CustInfo")]
        public string CustInfo { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_PA_InsertRobot";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}