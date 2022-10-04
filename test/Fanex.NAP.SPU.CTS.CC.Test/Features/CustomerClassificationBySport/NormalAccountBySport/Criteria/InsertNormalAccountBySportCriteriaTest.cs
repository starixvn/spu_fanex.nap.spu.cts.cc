namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Xunit;

    public class InsertNormalAccountBySportCriteriaTest
    {
        private readonly InsertNormalAccountBySportCriteria criteria;

        public InsertNormalAccountBySportCriteriaTest()
        {
            criteria = new InsertNormalAccountBySportCriteria();
        }

        [Fact]
        public void GetSettingKey_Any_ReturnRightSPName()
        {
            var expected = "CTS_DC_CustClassification_BySport_InsertNormalAccount";

            var spName = criteria.GetSettingKey();

            Assert.Equal(expected, spName);
        }
    }
}
