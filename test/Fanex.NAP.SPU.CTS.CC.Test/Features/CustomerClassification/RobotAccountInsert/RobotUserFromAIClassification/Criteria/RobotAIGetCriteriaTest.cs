namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Xunit;

    public class RobotAIGetCriteriaTest
    {
        private readonly RobotAIGetCriteria criteria;

        public RobotAIGetCriteriaTest()
        {
            criteria = new RobotAIGetCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_RobotClassification_Scan";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
