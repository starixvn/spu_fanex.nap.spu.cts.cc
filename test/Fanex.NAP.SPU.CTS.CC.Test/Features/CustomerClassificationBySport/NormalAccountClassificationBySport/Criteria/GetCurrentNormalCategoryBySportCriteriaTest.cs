namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountClassificationBySport.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria;
    using Xunit;

    public class GetCurrentNormalCategoryBySportCriteriaTest
    {
        private readonly GetCurrentNormalCategoryBySportCriteria criteria;

        public GetCurrentNormalCategoryBySportCriteriaTest()
        {
            criteria = new GetCurrentNormalCategoryBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_BySport_GetCurrentCategory";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
