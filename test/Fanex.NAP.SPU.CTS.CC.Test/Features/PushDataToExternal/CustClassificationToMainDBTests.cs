namespace Fanex.NAP.SPU.CTS.Tests.Features.PushDataToExternal
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class CustClassificationToMainDBTests
    {
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;

        public CustClassificationToMainDBTests()
        {
            sentryClient= Substitute.For<ISentryClient>();

            antiFraudCenterServices = Substitute.For<IAntiFraudCenterServices>();

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, sentryClient, antiFraudCenterServices);            
        }

        [Fact]
        public void PushExternalPackageByCustId_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custIDs = new List<long>() { 1, 2, 3 };
            var parameter = new PushDataToExternalEntity
            {
                ExternalBatchSize = 50,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = false,
                ServiceName = "Test"
            };

            // Act
            var exception = Record.Exception(() => custClassToMainDBService.PushExternalPackageByCustId(custIDs, parameter));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void PushExternalProblemAcountPackage_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custIDs = new List<long>() { 1, 2, 3 };
            var parameter = new PushDataToExternalEntity
            {
                ExternalBatchSize = 50,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = false,
                ServiceName = "Test"
            };

            // Act           
            var exception = Record.Exception(() => custClassToMainDBService.PushExternalProblemAcountPackage(custIDs, parameter));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void PushCustomerClassification_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var customerClass = new List<LatestClassificationEntity>()
            {
                new LatestClassificationEntity { CustId = 1, Category = 20200, CustomerClass = 10 }
            };

            // Act   
            custClassToMainDBDataAccess.UpdateCustomerClassification(customerClass).Returns(new Shares.Models.DBResult());

            var exception = Record.Exception(() => custClassToMainDBService.PushCustomerClassification(customerClass, 1, "Test"));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void PushCCBySportToMainDB_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custInfos = new List<InsertNormalAccountCustInfoEntity>()
            {
                new InsertNormalAccountCustInfoEntity (new CustInfoClassificationBySportEntity { CustID = 1, SportGroup = 1 })
            };

            // Act

            var exception = Record.Exception(() => custClassToMainDBService.PushCCBySportToMainDB(custInfos, "Test", 50));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void RetryPushCCBySportToMainDB_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custInfos = new List<GetCustomerClassificationBySportEntity>()
            {
                new GetCustomerClassificationBySportEntity { CustID = 1, SportID = 1, CustomerClass = 10 }
            };

            // Act

            var exception = Record.Exception(() => custClassToMainDBService.RetryPushCCBySportToMainDB(custInfos, 2, "Test"));

            // Assert
            Assert.Null(exception);
        }

    }
}