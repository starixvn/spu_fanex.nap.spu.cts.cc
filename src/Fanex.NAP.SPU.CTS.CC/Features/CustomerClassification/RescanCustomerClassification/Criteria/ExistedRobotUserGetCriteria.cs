namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ExistedRobotUserGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "custids")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "RobotList")]
        public string RobotList { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Customer_RobotCheckExist";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}