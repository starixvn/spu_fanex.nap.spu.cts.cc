namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Xunit;

    public class RobotUserGetCriteriaTest
    {
        private readonly RobotUserGetCriteria criteria;

        public RobotUserGetCriteriaTest()
        {
            criteria = new RobotUserGetCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_Customer_RobotListSel";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
