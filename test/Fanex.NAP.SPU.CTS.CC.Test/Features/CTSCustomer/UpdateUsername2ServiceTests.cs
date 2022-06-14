namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification;
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
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        public UpdateUsername2ServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, rescanClassificationService, custClassToMainDBService, antiFraudCenterServices, systemParameterDataAccess);
        }

        [Fact]
        public void GetCustWithNullUserName2_WhenSuccess_ReturnData()
        {
            var expectedResult = new List<GetCustIdModel>
            {
                new GetCustIdModel
                {
                    CTSCustID = 1,
                    CustID = 1,
                    CustSubID = 0,
                    UserName2 = "tester2"
                }
            };

            dataAccess.GetCustWithNullUserName2(10, 1).Returns(expectedResult);
            var result = ctsCustomerService.GetCustWithNullUserName2(10, 1);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetCustWithHavingUserName2_WhenSuccess_ReturnData()
        {
            var inputParam = new List<GetCustIdModel>();
            var expectedResult = new List<GetCustIdModel>
            {
                new GetCustIdModel
                {
                    CTSCustID = 1,
                    CustID = 1,
                    CustSubID = 0,
                    UserName2 = "tester2"
                }
            };

            dataAccess.GetCustWithHavingUserName2(inputParam).Returns(expectedResult);

            var result = ctsCustomerService.GetCustWithHavingUserName2(inputParam);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}