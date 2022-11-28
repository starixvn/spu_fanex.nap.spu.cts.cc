namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Xunit;

    public class UpdateLastScanTimeBySportCriteriaTest
    {
        private readonly UpdateLastScanBySportCriteria criteria;

        public UpdateLastScanTimeBySportCriteriaTest()
        {
            criteria = new UpdateLastScanBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_Realtime_UpdateLastScan";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
