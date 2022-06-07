namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.TWProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria;
    using Xunit;

    public class ScanSystemFormulaBetCustomerCompleteCriteriaTest
    {
        private readonly ScanSystemFormulaBetCustomerCompleteCriteria criteria;

        public ScanSystemFormulaBetCustomerCompleteCriteriaTest()
        {
            criteria = new ScanSystemFormulaBetCustomerCompleteCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "CTS_DC_TWProblemDetectionMonitor_Scan_Completed";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }

    }
}
