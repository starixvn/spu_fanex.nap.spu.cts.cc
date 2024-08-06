namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class CTSCustomerServiceTest
    {
        private readonly CTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess ctsCustomerDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;
        private readonly ISentryClient sentryClient;

        public CTSCustomerServiceTest()
        {
            sentryClient = Substitute.For<ISentryClient>();
            systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();
            ctsCustomerDataAccess = Substitute.For<ICTSCustomerDataAccess>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            
            ctsCustomerService = new CTSCustomerService(ctsCustomerDataAccess, sentryClient, custClassToMainDBService, systemParameterDataAccess);
        }

        [Fact]
        public void GetLastUpdatedTime_WhenSuccess_ReturnData()
        {
            // Arrange
            var expectedData = new LastUpdateValueModel();

            // Act
            ctsCustomerDataAccess.GetLastUpdatedTime().Returns(expectedData);

            var result = ctsCustomerService.GetLastUpdatedTime();

            // Assert
            result.Equals(expectedData);
        }

        [Fact]
        public void GetLastUpdatedTime_WhenException_ThrowException()
        {
            // Arrange

            // Act
            ctsCustomerDataAccess.GetLastUpdatedTime().Throws<Exception>();

            Action action = () => ctsCustomerService.GetLastUpdatedTime();

            // Assert
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void GetLastInsertCust_WhenSuccess_ReturnData()
        {
            // Arrange
            var expectedData = new LastScannedCustModel();

            // Act
            ctsCustomerDataAccess.GetLastInsertCust().Returns(expectedData);
            var result = ctsCustomerService.GetLastInsertCust();

            // Assert
            result.Equals(expectedData);
        }

        [Fact]
        public void GetLastInsertCust_WhenException_ThrowException()
        {
            // Arrange

            // Act
            ctsCustomerDataAccess.GetLastInsertCust().Throws<Exception>();

            Action action = () => ctsCustomerService.GetLastInsertCust();

            // Assert
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void GetCustomerFromMainDBForInsertNew_WhenSuccess_ReturnData()
        {
            // Arrange
            var numOfAccount = 1;
            var lastScannedCustID = 1;
            var expectedData = new InsertCustomerViewModel();

            // Act
            ctsCustomerDataAccess.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID).Returns(expectedData);

            var result = ctsCustomerService.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID);

            // Assert
            result.Equals(expectedData);
        }

        [Fact]
        public void GetCustSubFromMainDB_WhenSuccess_ReturnData()
        {
            // Arrange
            var custIDs = "1,2";
            var expectedData = new List<CustomerMainDBModel>();

            // Act
            ctsCustomerDataAccess.GetCustSubFromMainDB(custIDs).Returns(expectedData);

            var result = ctsCustomerService.GetCustSubFromMainDB(custIDs);

            // Assert
            result.Equals(expectedData);
        }

    }
}
