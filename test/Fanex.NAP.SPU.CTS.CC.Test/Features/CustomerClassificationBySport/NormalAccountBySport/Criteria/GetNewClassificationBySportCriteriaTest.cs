namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Xunit;

    public class GetNewClassificationBySportCriteriaTest
    {
        private readonly GetNewClassificationBySportCriteria criteria;

        public GetNewClassificationBySportCriteriaTest()
        {
            criteria = new GetNewClassificationBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_NormalAccountsClassification_BySport_GetNewClassification";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
