namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport;
    using Xunit;

    public class ScanDailyNormalAccountsClassificationBySportCriteriaTest
    {
        private readonly ScanDailyNormalAccountsClassificationBySportCriteria criteria;

        public ScanDailyNormalAccountsClassificationBySportCriteriaTest()
        {
            criteria = new ScanDailyNormalAccountsClassificationBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_BySport_Daily";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
