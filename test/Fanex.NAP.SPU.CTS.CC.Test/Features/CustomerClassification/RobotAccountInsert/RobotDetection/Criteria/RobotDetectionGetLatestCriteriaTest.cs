namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert.RobotDetection
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Criteria;
    using Xunit;

    public class RobotDetectionGetLatestCriteriaTest
    {
        private readonly RobotDetectionGetLatestCriteria criteria;

        public RobotDetectionGetLatestCriteriaTest()
        {
            criteria = new RobotDetectionGetLatestCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_RobotDetection_GetLatest";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
