namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Xunit;

    public class GetCustClassificationBySportByCustIDCriteriaTest
    {
        private readonly GetCustClassificationBySportByCustIDCriteria criteria;

        public GetCustClassificationBySportByCustIDCriteriaTest()
        {
            criteria = new GetCustClassificationBySportByCustIDCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_BySport_GetByCustIDs";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
