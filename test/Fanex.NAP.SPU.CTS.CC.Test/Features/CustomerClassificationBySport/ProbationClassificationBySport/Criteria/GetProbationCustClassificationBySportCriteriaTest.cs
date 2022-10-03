namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
    using Xunit;

    public class GetProbationCustClassificationBySportCriteriaTest
    {
        private readonly GetProbationCustClassificationBySportCriteria criteria;

        public GetProbationCustClassificationBySportCriteriaTest()
        {
            criteria = new GetProbationCustClassificationBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_BySport_ProbationScan_Get";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
