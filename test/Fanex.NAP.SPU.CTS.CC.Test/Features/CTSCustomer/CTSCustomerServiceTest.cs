namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

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

        [Fact]
        public void InsertNewCustomer_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new InsertCustomerViewModel
            {
                CustomerInfos = new List<CustomerMainDBModel>() { new CustomerMainDBModel { CustID = 1 } },
                LastInsertCustID = 1
            };

            var paras = new InsertNewCustRescanEntity 
            {
                InternalBatchSize = 50,
                ExternalBatchSize = 50,
                IsWriteLog = true,
                IsInsertMissingCust = false,
                InsertVIPURL = string.Empty,
                InsertBAURL = string.Empty,
                serviceName = "InsertNewCustomer"
            };

            var expectedData = new List<InsertNewCustomerResultModel>()
            {
                new InsertNewCustomerResultModel 
                {
                     CustID = 1, CTSCustID = 1, RoleID = 1
                }
            };

            // Act
            ctsCustomerDataAccess.InsertNewCTSCustomer(Arg.Any<IEnumerable<CustomerMainDBModel>>(), true).Returns(expectedData);

            custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>()).Returns(true);

            var result = ctsCustomerService.InsertNewCustomer(true, data, paras);


            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void UpdateCustomer_WhenSuccess_ReturnNotException()
        {           
            // Arrange
            var custs = new TranformCustModel {
                CustIdList = "1,2",
                LastCustId = 2,
                LastUpdatedTime = DateTime.Now
            };

            var sourceType = UpdateCustomer.FromCustomer;
            var isWriteLog = true;

            // Act 
            Action action = () => ctsCustomerService.UpdateCustomer(custs, sourceType, isWriteLog);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void GetCustInfoToUpdate_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custs = new TranformCustModel();
            var sourceType = UpdateCustomer.FromCustomer;
            var isWriteLog = true;

            // Act
            Action action = () => ctsCustomerService.GetCustInfoToUpdate(custs, sourceType, isWriteLog, out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateUsername2_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custInfos = new List<CustNullUserName2Model>()
            { 
                new CustNullUserName2Model { CustID = 1, CTSCustID = 1, CustSubID = 0, UserName2 = "abc"} 
            };

            // Act
            Action action = () => ctsCustomerService.UpdateUsername2(custInfos);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateUserName2Package_WhenSuccess_ReturnNotException()
        {
            // Arrange
            ulong lastCTSCustId = 1;
            var isCustSub = true;
            var rowSize = 50;
            var expectedData = new ScannedCustModel() { CustList = new List<CustNullUserName2Model>()
            { 
                new CustNullUserName2Model {  CustID = 1} },
                LastCTSCustID = 2 
            };

            // Act
            ctsCustomerService.GetCustWithNullUserName2(rowSize, lastCTSCustId, isCustSub).Returns(expectedData);

            Action action = () => ctsCustomerService.UpdateUserName2Package(ref lastCTSCustId, isCustSub, rowSize);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void InsertReactivedCustomerLog_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var time = DateTime.Now;

            // Act
            Action action = () => ctsCustomerService.InsertReactivedCustomerLog(time, time, time, time);

            // Assert
            action.Should().NotThrow<Exception>();
        }
    }
}
