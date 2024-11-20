namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RescanCustomerClassificationTest
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly IRescanCustClassificationService rescanCustClassificationService;

        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;
        private readonly ISentryClient sentryClient;

        public RescanCustomerClassificationTest()
        {
            sentryClient = Substitute.For<ISentryClient>();
            baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();

            rescanCustClassificationDataAccess = Substitute.For<IRescanCustClassificationDataAccess>();
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();

            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            rescanCustClassificationService = new RescanCustClassificationService(baseDataAccess, rescanCustClassificationDataAccess,
                problemAccountDataAccess, custClassToMainDBService, sentryClient);
        }

        [Fact]
        public void PushExternalProblemAccounts_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custIds = new List<long>() { 1, 2 };
            var paras = new PushDataToExternalEntity
            {
                IsUpdateDangerToMainDB = true,
                ExternalBatchSize = 50,
                ServiceName = "Test",
                IsSyncAFCData = false,
                AFCInsertAccountURL = string.Empty,
                AFCDeleteAccountURL = string.Empty,
            };

            // Act
            Action action = () => rescanCustClassificationService.PushExternalProblemAccounts(custIds, paras);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void RescanCustomerClassification_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custIds = new List<long>() { 1, 2 };
            var rescanParas = new RescanParametersEntity()
            {
                IsToQueue = false,
                FunctionId = FunctionId.InactivateNewCustomer,
                RescanNormalType = RescanNormalType.Auto_Customer,
                InternalBatchSize = 50,
                ServiceName = "Test",
                PushDataExternal = new PushDataToExternalEntity
                {
                    IsUpdateDangerToMainDB = true,
                    ExternalBatchSize = 50,
                    ServiceName = "Test",
                    IsSyncAFCData = false,
                    AFCInsertAccountURL = string.Empty,
                    AFCDeleteAccountURL = string.Empty,
                }
            };

            var expectedData = new GetRescanCustomerClassificationModel()
            {
                CustInfos = new List<GetRescanCustomerClassificationEntity>
                {
                    new GetRescanCustomerClassificationEntity {InputFlowID = (int) CCInputFlow.RescanNormal},
                    new GetRescanCustomerClassificationEntity {InputFlowID = (int) CCInputFlow.RescanPACategory},
                    new GetRescanCustomerClassificationEntity {InputFlowID = (int) CCInputFlow.RescanPotential},
                }
            };

            // Act
            rescanCustClassificationDataAccess.GetRoleCustomer(Arg.Any<string>()).Returns(new List<RoleCustomerEntity> { new RoleCustomerEntity { RoleID = 2 } });

            baseDataAccess.GetRescanCustomerClassification(Arg.Any<string>()).Returns(expectedData);

            baseDataAccess.InsertCustomerClassification(Arg.Any<CCInputFlow>(), Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel());

            custClassToMainDBService.PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), rescanParas.PushDataExternal).Returns(true);

            var result = rescanCustClassificationService.RescanCustomerClassification(custIds, rescanParas);

            // Assert
            Assert.True(result);

        }


    }
}