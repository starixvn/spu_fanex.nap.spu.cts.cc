namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Xunit;

    public class UpdateLastScanTimeBySportCriteriaTest
    {
        private readonly UpdateLastScanTimeBySportCriteria criteria;

        public UpdateLastScanTimeBySportCriteriaTest()
        {
            criteria = new UpdateLastScanTimeBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_BySport_Realtime_UpdateLastScanTime";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
