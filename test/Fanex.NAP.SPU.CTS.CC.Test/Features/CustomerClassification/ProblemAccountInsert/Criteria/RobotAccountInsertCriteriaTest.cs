namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class RobotAccountInsertCriteriaTest
    {
        private readonly RobotAccountInsertCriteria criteria;

        public RobotAccountInsertCriteriaTest()
        {
            criteria = new RobotAccountInsertCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_PA_InsertRobot";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
