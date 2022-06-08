namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class BetLimitByEvidenceUpdateCriteriaTest
    {
        private readonly BetLimitByEvidenceUpdateCriteria criteria;

        public BetLimitByEvidenceUpdateCriteriaTest()
        {
            criteria = new BetLimitByEvidenceUpdateCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "NAP_BLControl_EvidenceFlagged";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
