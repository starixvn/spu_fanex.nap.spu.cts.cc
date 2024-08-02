namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.DailyDangerousClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;  
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class DailyDangerousClassificationServiceTest
    {        
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private readonly IDailyDangerousClassificationDataAccess dangerousCCDataAccess;
        private readonly IDailyDangerousClassificationService dailyDangerousCCService;

        private readonly IBaseCustomerClassificationDataAccess baseCustomerClassificationDataAccess;
        private readonly IBaseCustomerClassificationService baseCustomerClassificationService;

        private readonly ISentryClient sentryClient;

        public DailyDangerousClassificationServiceTest()
        {
            sentryClient = Substitute.For<ISentryClient>();
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
          
            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, null);
      
            baseCustomerClassificationDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            baseCustomerClassificationService = new BaseCustomerClassificationService(baseCustomerClassificationDataAccess, sentryClient);           

            dangerousCCDataAccess = Substitute.For<IDailyDangerousClassificationDataAccess>();
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
            baseCustomerClassificationDataAccess.When(x => x.InsertCustomerClassification(CCInputFlow.Potential, Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel()));

            baseCustomerClassificationService.InsertCustomerClassification(CCInputFlow.Potential, Arg.Any<InsertCustomerClassificationModel>(), paras.ServiceName).Returns(new ResultInsertCustomerClassificationModel());                       
            
            var result = dailyDangerousCCService.InsertDangerousClassification(dangerousCCs, isLicensee, paras);

            // Assert
            result.Should().BeTrue();
        }

    }
}
