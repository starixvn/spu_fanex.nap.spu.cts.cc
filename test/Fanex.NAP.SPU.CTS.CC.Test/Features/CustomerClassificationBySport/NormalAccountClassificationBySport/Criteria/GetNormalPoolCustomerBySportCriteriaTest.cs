namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountClassificationBySport.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria;
    using Xunit;

    public class GetNormalPoolCustomerBySportCriteriaTest
    {
        private readonly GetNormalPoolCustomerBySportCriteria criteria;

        public GetNormalPoolCustomerBySportCriteriaTest()
        {
            criteria = new GetNormalPoolCustomerBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_BySportNormalClassification_NormalPool_Get";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
