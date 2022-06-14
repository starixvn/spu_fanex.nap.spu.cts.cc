namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class ProblemAccountGetCriteriaTest
    {
        private readonly ProblemAccountGetCriteria criteria;

        public ProblemAccountGetCriteriaTest()
        {
            criteria = new ProblemAccountGetCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_PA_ReScan";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
