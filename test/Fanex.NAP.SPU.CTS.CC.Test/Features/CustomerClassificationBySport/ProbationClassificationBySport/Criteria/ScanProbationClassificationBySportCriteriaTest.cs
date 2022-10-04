namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport;
    using Xunit;

    public class ScanProbationClassificationBySportCriteriaTest
    {
        private readonly ScanProbationClassificationBySportCriteria criteria;

        public ScanProbationClassificationBySportCriteriaTest()
        {
            criteria = new ScanProbationClassificationBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_BySport_Probation";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
