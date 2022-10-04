namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Xunit;

    public class UpdateAppliedStatusClassificationBySportCriteriaTest
    {
        private readonly UpdateAppliedStatusClassificationBySportCriteria criteria;

        public UpdateAppliedStatusClassificationBySportCriteriaTest()
        {
            criteria = new UpdateAppliedStatusClassificationBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_BySport_UpdateAppliedStatus";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
