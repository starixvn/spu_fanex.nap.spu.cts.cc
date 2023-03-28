namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.TWRobotImperva.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Criteria;
    using Xunit;

    public class TWRobotImpervaGetCriteriaTest
    {
        private readonly TWRobotImpervaGetCriteria criteria;

        public TWRobotImpervaGetCriteriaTest()
        {
            criteria = new TWRobotImpervaGetCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_RobotImperva_Get";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
