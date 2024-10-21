namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System.Collections.Generic;
    using Xunit;

    public class UpdateUsername2ServiceTests
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;

        public UpdateUsername2ServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, custClassToMainDBService);
        }

        [Fact]
        public void GetCustWithNullUserName2_WhenSuccess_ReturnData()
        {
            var expectedResult = new ScannedCustModel
            {
                CustList = new List<CustNullUserName2Model>
                {
                    new CustNullUserName2Model
                    {
                        CTSCustID = 1,
                        CustID = 1,
                        CustSubID = 0,
                        UserName2 = "tester2"
                    }
                },
                LastCTSCustID = 1
            };             

            dataAccess.GetCustWithNullUserName2(10, 1, true).Returns(expectedResult);

            var result = ctsCustomerService.GetCustWithNullUserName2(10, 1, true);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetCustWithHavingUserName2_WhenSuccess_ReturnData()
        {
            var inputParam = new List<CustNullUserName2Model>();
            var expectedResult = new List<CustNullUserName2Model>
            {
                new CustNullUserName2Model
                {
                    CTSCustID = 1,
                    CustID = 1,
                    CustSubID = 0,
                    UserName2 = "tester2"
                }
            };

            dataAccess.GetCustWithHavingUserName2(inputParam, false).Returns(expectedResult);

            var result = ctsCustomerService.GetCustWithHavingUserName2(inputParam, false);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}