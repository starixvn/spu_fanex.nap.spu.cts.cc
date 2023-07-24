namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountClassificationBySport.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport.Criteria;
    using Xunit;

    public class ClassifyNormalPoolCustomerBySportCriteriaTest
    {
        private readonly ClassifyNormalPoolCustomerBySportCriteria criteria;

        public ClassifyNormalPoolCustomerBySportCriteriaTest()
        {
            criteria = new ClassifyNormalPoolCustomerBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_BySportNormalClassification_NormalPool_Classify";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
