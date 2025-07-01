namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class CTSCustomerServiceTest
    {
        private readonly CTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess ctsCustomerDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        public CTSCustomerServiceTest()
        {
            ctsCustomerDataAccess = Substitute.For<ICTSCustomerDataAccess>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            ctsCustomerService = new CTSCustomerService(ctsCustomerDataAccess, custClassToMainDBService);

            LogExtension.ConfigByPassForUnitTest();
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
        public void GetCustSubFromMainDB_WhenSuccess_ReturnData()
        {
            // Arrange
            var custIDs = "1,2";
            var expectedData = new List<CustomerMainDBEntity>();

            // Act
            ctsCustomerDataAccess.GetCustSubFromMainDBByCustIDs(custIDs).Returns(expectedData);

            var result = ctsCustomerService.GetCustSubFromMainDBByCustIDs(custIDs);

            // Assert
            result.Equals(expectedData);
        }

        [Fact]
        public void UpdateCustomer_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custs = new TranformCustModel
            {
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
            var expectedData = new ScannedCustModel()
            {
                CustList = new List<CustNullUserName2Model>()
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
