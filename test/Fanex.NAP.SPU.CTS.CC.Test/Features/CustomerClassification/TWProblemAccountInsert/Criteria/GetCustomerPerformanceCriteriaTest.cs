namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.TWProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria;
    using Xunit;

    public class GetCustomerPerformanceCriteriaTest
    {
        private readonly GetCustomerPerformanceCriteria criteria;

        public GetCustomerPerformanceCriteriaTest()
        {
            criteria = new GetCustomerPerformanceCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_CustomerClassification_GetPerformance";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
