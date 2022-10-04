namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Xunit;

    public class NormalAccountsClassificationBySportCategorizeRealtimeCriteriaTest
    {
        private readonly NormalAccountsClassificationBySportCategorizeRealtimeCriteria criteria;

        public NormalAccountsClassificationBySportCategorizeRealtimeCriteriaTest()
        {
            criteria = new NormalAccountsClassificationBySportCategorizeRealtimeCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_BySport_RealTime";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
