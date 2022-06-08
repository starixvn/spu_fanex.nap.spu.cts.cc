namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert.Criteria
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Criteria;
    using Xunit;

    public class CheckNewPAAssociationCustomerForRollbackCriteriaTest
    {
        private readonly CheckNewPAAssociationCustomerForRollbackCriteria criteria;

        public CheckNewPAAssociationCustomerForRollbackCriteriaTest()
        {
            criteria = new CheckNewPAAssociationCustomerForRollbackCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_Arc_CustomerAssociation_RollBack_CheckByNewPA";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
