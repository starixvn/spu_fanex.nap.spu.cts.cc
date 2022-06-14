namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.TWProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria;
    using Xunit;

    public class ScanSystemFormulaBetCustomerCriteriaTest
    {
        private readonly ScanSystemFormulaBetCustomerCriteria criteria;

        public ScanSystemFormulaBetCustomerCriteriaTest()
        {
            criteria = new ScanSystemFormulaBetCustomerCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            // Arrange
            var expected = "CTS_DC_TWProblemDetectionMonitor_Scan";

            // Act
            var spName = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, spName);
        }
    }
}
