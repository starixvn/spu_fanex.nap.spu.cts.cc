namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.DailyDangerousClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;  
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System.Collections.Generic;
    using Xunit;

    public class DailyDangerousClassificationServiceTest
    {        
        private readonly IProblemAccountDataAccess problemAccountDataAccess;

        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private readonly IDailyDangerousClassificationDataAccess dangerousCCDataAccess;
        private readonly IDailyDangerousClassificationService dailyDangerousCCService;

        private readonly IBaseCustomerClassificationDataAccess baseCustomerClassificationDataAccess;
        private readonly ISentryClient sentryClient;

        public DailyDangerousClassificationServiceTest()
        {
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>(); 

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, null);

            dangerousCCDataAccess = Substitute.For<IDailyDangerousClassificationDataAccess>();
            baseCustomerClassificationDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();

            dailyDangerousCCService = new DailyDangerousClassificationService(dangerousCCDataAccess, problemAccountDataAccess, baseCustomerClassificationDataAccess, custClassToMainDBService, sentryClient);
        }

        [Fact]
        public void GetDangerousClassification_WhenSuccess_ReturnDangerousToCC()
        {
            // Arrange
            var isLicensee = false;
            var expectedDangerousCCs = new DangerousToCCModel();

            // Act
            dangerousCCDataAccess.GetDangerousClassification(isLicensee).Returns(expectedDangerousCCs);

            var result = dailyDangerousCCService.GetDangerousClassification(isLicensee);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDangerousCCs);
        }

        [Fact]
        public void InsertDangerousClassification_WhenSuccess_ReturnData()
        {
            // Arrange
            var isLicensee = false;
            var dangerousCCs = new List<DangerousToCCEntity>()
            {
                new DangerousToCCEntity { CustID = 1, RoleID = 1, CategoryID = 65, SubscriberID = 6, CTSCustID = 1, CategoryGroup = 50 }
            };
            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = 500,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = false,
                ServiceName = "DailyDangerousClassificationServiceTest"
            };

            // Act
            var result = dailyDangerousCCService.InsertDangerousClassification(dangerousCCs, isLicensee, paras);

            // Assert
            result.Should().BeTrue();

        }

    }
}
