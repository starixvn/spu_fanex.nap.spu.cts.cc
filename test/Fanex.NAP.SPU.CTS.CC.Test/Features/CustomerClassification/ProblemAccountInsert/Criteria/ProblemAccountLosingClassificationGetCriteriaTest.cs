namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class ProblemAccountLosingClassificationGetCriteriaTest
    {
        private readonly ProblemAccountLosingClassificationGetCriteria criteria;

        public ProblemAccountLosingClassificationGetCriteriaTest()
        {
            criteria = new ProblemAccountLosingClassificationGetCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_ProblemAccountsClassification_Daily";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
