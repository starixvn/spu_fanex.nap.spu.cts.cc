namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class OldProblemAccountRemoveCriteriaTest
    {
        private readonly OldProblemAccountRemoveCriteria criteria;

        public OldProblemAccountRemoveCriteriaTest()
        {
            criteria = new OldProblemAccountRemoveCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_PA_ClearOld";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
