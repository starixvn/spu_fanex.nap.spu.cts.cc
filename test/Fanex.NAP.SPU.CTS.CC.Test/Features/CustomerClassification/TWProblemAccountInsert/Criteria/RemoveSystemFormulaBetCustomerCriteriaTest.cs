namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.TWProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria;   
    using Xunit;

    public class RemoveSystemFormulaBetCustomerCriteriaTest
    {
        private readonly RemoveSystemFormulaBetCustomerCriteria criteria;

        public RemoveSystemFormulaBetCustomerCriteriaTest()
        {
            criteria = new RemoveSystemFormulaBetCustomerCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_TWProblemDetectionMonitor_Remove";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
