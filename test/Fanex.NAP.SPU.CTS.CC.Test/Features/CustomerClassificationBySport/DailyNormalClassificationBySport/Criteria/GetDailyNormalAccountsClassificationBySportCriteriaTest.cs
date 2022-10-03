namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport;
    using Xunit;

    public class GetDailyNormalAccountsClassificationBySportCriteriaTest
    {
        private readonly GetDailyNormalAccountsClassificationBySportCriteria criteria;

        public GetDailyNormalAccountsClassificationBySportCriteriaTest()
        {
            criteria = new GetDailyNormalAccountsClassificationBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_BySport_DailyScan_GetNormal";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
