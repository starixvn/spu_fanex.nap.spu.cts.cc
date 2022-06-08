namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class ProblemAccountInsertCriteriaTest
    {
        private readonly ProblemAccountInsertCriteria criteria;

        public ProblemAccountInsertCriteriaTest()
        {
            criteria = new ProblemAccountInsertCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_PA_Insert";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
