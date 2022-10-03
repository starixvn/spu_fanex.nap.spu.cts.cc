namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
    using Xunit;

    public class UpdateScanDateProbationClassificationBySportCriteriaTest
    {
        private readonly UpdateScanDateProbationClassificationBySportCriteria criteria;

        public UpdateScanDateProbationClassificationBySportCriteriaTest()
        {
            criteria = new UpdateScanDateProbationClassificationBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_BySport_ProbationScan_UpdateScanDate";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
