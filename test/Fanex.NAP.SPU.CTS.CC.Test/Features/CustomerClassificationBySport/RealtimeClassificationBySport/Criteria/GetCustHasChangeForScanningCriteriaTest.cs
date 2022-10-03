namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Xunit;

    public class GetCustHasChangeForScanningCriteriaTest
    {
        private readonly GetCustHasChangeForScanningCriteria criteria;

        public GetCustHasChangeForScanningCriteriaTest()
        {
            criteria = new GetCustHasChangeForScanningCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_BySport_RealTime_CheckChanges";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
