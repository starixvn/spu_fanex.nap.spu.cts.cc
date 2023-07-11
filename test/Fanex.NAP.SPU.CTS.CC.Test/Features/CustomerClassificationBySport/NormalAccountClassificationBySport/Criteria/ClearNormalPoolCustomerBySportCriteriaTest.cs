namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountClassificationBySport.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria;
    using Xunit;

    public class ClearNormalPoolCustomerBySportCriteriaTest
    {
        private readonly ClearNormalPoolCustomerBySportCriteria criteria;

        public ClearNormalPoolCustomerBySportCriteriaTest()
        {
            criteria = new ClearNormalPoolCustomerBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_BySportNormalClassification_NormalPool_Clear";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
