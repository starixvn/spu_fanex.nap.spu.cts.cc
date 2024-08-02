namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.AffectedDownlineCustomerClassification
{
    using NSubstitute;
    using Xunit;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using System.Collections.Generic;
    using Sentry;

    public class AffectedDownlineCustomerClassificationServiceTest
    {
        private readonly IAffectedDownlineCustomerClassificationDataAccess affectedDownlineCustomerClassificationDataAccess;
        private readonly IAffectedDownlineCustomerClassificationService affectedDownlineCustomerClassificationService;
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IBaseCustomerClassificationDataAccess baseCustomerClassificationDataAccess;
        private readonly IBaseCustomerClassificationService baseCustomerClassificationService;
        private readonly ISentryClient sentryClient;

        public AffectedDownlineCustomerClassificationServiceTest()
        {
            sentryClient = Substitute.For<ISentryClient>();

            affectedDownlineCustomerClassificationDataAccess = Substitute.For<IAffectedDownlineCustomerClassificationDataAccess>();
            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();

            baseCustomerClassificationDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            baseCustomerClassificationService = new BaseCustomerClassificationService(baseCustomerClassificationDataAccess, sentryClient);

            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, null);

            affectedDownlineCustomerClassificationService = new AffectedDownlineCustomerClassificationService(affectedDownlineCustomerClassificationDataAccess,
                baseCustomerClassificationDataAccess, problemAccountDataAccess, 
                custClassToMainDBService, null, Substitute.For<IRescanCustClassificationService>(), sentryClient);
        }

        [Fact]
        public void GetCustomerClassificationFromQueue_WhenSuccess_DoesNotThrowException()
        {
            // Arrange

            // Act
            affectedDownlineCustomerClassificationDataAccess.GetCustomerCategoryFromQueue();

            var exception = Record.Exception(() => affectedDownlineCustomerClassificationService.GetCustomerClassificationFromQueue());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void CustomerClassification_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var paramaters = ExpectedRescanParameters();

            var expectedData = ExpectedDataFromQueue();

            // Act
            affectedDownlineCustomerClassificationDataAccess.GetCustomerCategoryFromQueue().Returns(expectedData);

            affectedDownlineCustomerClassificationService.GetCustomerClassificationFromQueue();

            baseCustomerClassificationDataAccess.When(x => x.InsertCustomerClassification(CCInputFlow.VVIP, Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel()));

            baseCustomerClassificationService.InsertCustomerClassification(CCInputFlow.VVIP, Arg.Any<InsertCustomerClassificationModel>(), "").Returns(new ResultInsertCustomerClassificationModel());

            var exception = Record.Exception(() => affectedDownlineCustomerClassificationService.CustomerClassification(paramaters));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void CompleteCustomerClassificationFromQueue_WhenSuccess_ReturnNotException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => affectedDownlineCustomerClassificationService.CompleteCustomerClassificationFromQueue());
          
            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void InsertVVIP_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var rescanParamaters = ExpectedRescanParameters();
            var custInfo = new VVIPCustomerInsertEntity
            {
                CustInfo = new List<VVIPCategoryEntity>() { new VVIPCategoryEntity {  CustID = 1, CTSCustID = 1, RoleID = 1, SubscriberID = 6, CreatedBy = 8} },
                FromFlow = FromFlow.CTSServices,
                IsAffectDownline = false,
                QueueID = null,
                UplineRoleID = null
            };

            // Act
            baseCustomerClassificationDataAccess.When(x => x.InsertCustomerClassification(CCInputFlow.VVIP, Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel()));

            baseCustomerClassificationService.InsertCustomerClassification(CCInputFlow.VVIP, Arg.Any<InsertCustomerClassificationModel>(), "").Returns(new ResultInsertCustomerClassificationModel());

            var exception = Record.Exception(() => affectedDownlineCustomerClassificationService.InsertVVIP(rescanParamaters, custInfo));
          
            // Assert
            Assert.Null(exception);
        }

        private CustClassificationFromQueueResultEntity ExpectedDataFromQueue()
        {
            return new CustClassificationFromQueueResultEntity()
            {
                QueueID = 1,
                ActionType = 1,
                CustomerClassificationQueue = new List<CustClassificationFromQueueEntity>() {
                    new CustClassificationFromQueueEntity
                    {
                         CustID = 1, CTSCustID = 1, CategoryID = 30300, ActiveDays = 1, BetCount = 1
                    }
                },
                LastDownlineCTSCustID = 1,
                UplineRoleID = 1
            };
        }

        private RescanParametersEntity ExpectedRescanParameters()
        {
            return new RescanParametersEntity()
            {
                IsToQueue = false,
                FunctionId = FunctionId.DownlineCustomerClassification,
                RescanNormalType = RescanNormalType.Auto_AfftectedDownline,
                InternalBatchSize = 50,
                ServiceName = "Affected Downline Customer Classification",
                PushDataExternal = new PushDataToExternalEntity
                {
                    IsUpdateDangerToMainDB = true,
                    ExternalBatchSize = 50,
                    ServiceName = "Affected Downline Customer Classification",
                    IsSyncAFCData = false,
                    AFCInsertAccountURL = string.Empty,
                    AFCDeleteAccountURL = string.Empty
                }
            };
        }
    }
}
