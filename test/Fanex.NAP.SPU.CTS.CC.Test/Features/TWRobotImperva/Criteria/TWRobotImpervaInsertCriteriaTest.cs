namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.TWRobotImperva.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Criteria;
    using Xunit;

    public class TWRobotImpervaInsertCriteriaTest
    {
        private readonly TWRobotImpervaInsertCriteria criteria;

        public TWRobotImpervaInsertCriteriaTest()
        {
            criteria = new TWRobotImpervaInsertCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expect = "CTS_DC_RobotImperva_Insert";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expect, spName);
        }
    }
}
