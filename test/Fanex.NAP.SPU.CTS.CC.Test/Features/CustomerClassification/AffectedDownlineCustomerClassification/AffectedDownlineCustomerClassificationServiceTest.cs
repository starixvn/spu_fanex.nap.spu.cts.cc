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

    public class AffectedDownlineCustomerClassificationServiceTest
    {
        private readonly IAffectedDownlineCustomerClassificationDataAccess affectedDownlineCustomerClassificationDataAccess;
        private readonly IAffectedDownlineCustomerClassificationService affectedDownlineCustomerClassificationService;
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IRescanCustClassificationService rescanCustClassificationService;

        public AffectedDownlineCustomerClassificationServiceTest()
        {
            affectedDownlineCustomerClassificationDataAccess = Substitute.For<IAffectedDownlineCustomerClassificationDataAccess>();
            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, null);

            affectedDownlineCustomerClassificationService = new AffectedDownlineCustomerClassificationService(affectedDownlineCustomerClassificationDataAccess,
                Substitute.For<IBaseCustomerClassificationDataAccess>(),
               Substitute.For<IProblemAccountDataAccess>(), custClassToMainDBService, null, rescanCustClassificationService, null);
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
            var paramaters = new RescanParametersEntity()
            {
                IsToQueue = false,
                FunctionId = FunctionId.DownlineCustomerClassification,
                RescanNormalType = RescanNormalType.Auto_AfftectedDownline,
                InternalBatchSize = 50,
                ServiceName = "AffectedDownlineCustomerClassification",
                PushDataExternal = new PushDataToExternalEntity
                {
                    IsUpdateDangerToMainDB = true,
                    ExternalBatchSize = 50,
                    ServiceName = "",
                    IsSyncAFCData = false,
                    AFCInsertAccountURL = string.Empty,
                    AFCDeleteAccountURL = string.Empty
                }
            };
            var expectedData = ExpectedDataFromQueue();

            // Act
            affectedDownlineCustomerClassificationDataAccess.GetCustomerCategoryFromQueue().Returns(expectedData);

            affectedDownlineCustomerClassificationService.GetCustomerClassificationFromQueue();

            var exception = Record.Exception(() => affectedDownlineCustomerClassificationService.CustomerClassification(paramaters));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void CompleteCustomerClassificationFromQueue_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var expectedData = ExpectedDataFromQueue();

            // Act
            var result = affectedDownlineCustomerClassificationService.CompleteCustomerClassificationFromQueue();

            // Assert

        }

        [Fact]
        public void InsertVVIP_WhenSuccess_ReturnNotException()
        {
            // Arrange


            // Act


            // Assert

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
    }
}
