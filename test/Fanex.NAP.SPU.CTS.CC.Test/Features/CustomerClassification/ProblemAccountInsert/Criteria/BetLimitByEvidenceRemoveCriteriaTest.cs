namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class BetLimitByEvidenceRemoveCriteriaTest
    {
        private readonly BetLimitByEvidenceRemoveCriteria criteria;

        public BetLimitByEvidenceRemoveCriteriaTest()
        {
            criteria = new BetLimitByEvidenceRemoveCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "NAP_BLControl_EvidenceUnflagged";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
