namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class ProblemAccountRemoveCriteriaTest
    {
        private readonly ProblemAccountRemoveCriteria criteria;

        public ProblemAccountRemoveCriteriaTest()
        {
            criteria = new ProblemAccountRemoveCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_PA_Remove";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
