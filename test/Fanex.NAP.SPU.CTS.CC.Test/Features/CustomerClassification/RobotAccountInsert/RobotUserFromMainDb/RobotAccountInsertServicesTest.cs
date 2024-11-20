namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.RobotTWInsertConsts;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotAccountInsertServicesTest
    {
        private readonly IRobotTWInsertDataAccess robotAccountInsertDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService _custClassToMainDBService;   
        private readonly IDataLogTrackingServices _dataLogTrackingServices;
        private readonly IBaseCustomerClassificationDataAccess _baseDataAccess;
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ICustomerClassificationAgencyService _ccAgencyService;
        private readonly ISentryClient sentryClient;

        private readonly IRobotTWInsertServices _service;

        public RobotAccountInsertServicesTest()
        {
            robotAccountInsertDataAccess = Substitute.For<IRobotTWInsertDataAccess>();
            systemParameterServices = Substitute.For<ISystemParameterServices>();           
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _dataLogTrackingServices = Substitute.For<IDataLogTrackingServices>();
            sentryClient = Substitute.For<ISentryClient>();
            _baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            _ccAgencyService = Substitute.For<ICustomerClassificationAgencyService>();

            _service = new RobotTWInsertServices(
                  robotAccountInsertDataAccess
                , systemParameterServices
                , _custClassToMainDBService
                , _dataLogTrackingServices
                , _baseDataAccess
                , problemAccountDataAccess
                , _ccAgencyService
                , sentryClient);
        }

        [Fact]
        public void GetLastTimespan_WhenSuccess()
        {
            // Act
            var result = _service.GetLastTimespan();

            // Assert
            Assert.IsAssignableFrom<IRobotTWInsertServices>(result);
        }

        [Fact]
        public void GetRobotUser_WhenSuccess()
        {
            // Act
            var result = _service.GetTWRobotUser();

            // Assert
            Assert.IsAssignableFrom<IRobotTWInsertServices>(result);
        }


        [Fact]
        public void InsertTWRobot_WhenSuccess_ReturnData()
        {
            // Arrange
            var robotTWs = new List<TWRobotUserEntity> { };

            var expectedResult = new List<long>();

            // Act
            var result = _service.InsertTWRobotPackage(1,"", null);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]     
        [InlineData(false)]
        public void InsertTWRobotPackage_WhenSuccess_ReturnTrue(bool isSuccess)
        {
            // Arrange
            var robotInfoField = TestExtension.GetPrivateFieldInfo(typeof(RobotTWInsertServices), PrivateFieldName.RobotInfo);
            robotInfoField.SetValue(_service, new TWRobotUserViewEntity
            {
                RobotUsers = new List<TWRobotUserEntity>
                {
                    new TWRobotUserEntity(),
                    new TWRobotUserEntity(),
                    new TWRobotUserEntity(),
                },
            });

            var robotCategoryIdField = TestExtension.GetPrivateFieldInfo(typeof(RobotTWInsertServices), PrivateFieldName.RobotCategoryId);
            robotCategoryIdField.SetValue(_service, 999);

            var pushConfig = new PushDataToExternalEntity();

            // Act
            _baseDataAccess
                .InsertCustomerClassification(Arg.Any<CCInputFlow>(), Arg.Any<InsertCustomerClassificationModel>())
                .Returns(new ResultInsertCustomerClassificationModel()
                {
                    CustInfos = new List<CustInfoClassificationEntity>() { new CustInfoClassificationEntity() }
                });

            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushConfig).Returns(isSuccess);

            var result = _service.InsertTWRobotPackage(999, "", pushConfig);

            // Assert
            _dataLogTrackingServices.Received(1).LogInputData(Arg.Any<LogInputViewEntity>(), FunctionId.RobotUserJob, Arg.Any<string>());

            Assert.True(isSuccess == result);
        }

        [Fact]
        public void UpdateLastTimespan_WhenSuccess()
        {
            // Arrange
            var robotInfoField = TestExtension.GetPrivateFieldInfo(typeof(RobotTWInsertServices), PrivateFieldName.RobotInfo);
            robotInfoField.SetValue(_service, new TWRobotUserViewEntity
            {
                RobotUsers = new List<TWRobotUserEntity> { new TWRobotUserEntity() },
                MaxTimestamp = 999
            });

            var robotCategoryIdField = TestExtension.GetPrivateFieldInfo(typeof(RobotTWInsertServices), PrivateFieldName.RobotCategoryId);
            robotCategoryIdField.SetValue(_service, 999);

            // Act
            systemParameterServices.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            _service.UpdateLastTimespan();

            // Assert
            systemParameterServices.Received(1).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());
        }

        [Fact]
        public void GetRobotUserCategoryId_WhenSuccess()
        {
            // Arrange
            var robotInfoField = TestExtension.GetPrivateFieldInfo(typeof(RobotTWInsertServices), PrivateFieldName.RobotInfo);
            robotInfoField.SetValue(_service, new TWRobotUserViewEntity
            {
                RobotUsers = new List<TWRobotUserEntity> { new TWRobotUserEntity() },
                MaxTimestamp = 999
            });

            // Act
            _baseDataAccess.GetCategoryByCategoryTypeName(Arg.Any<string>()).Returns(999);

            _service.GetRobotUserCategoryId("TEST");

            // Assert
            _baseDataAccess.Received(1).GetCategoryByCategoryTypeName(Arg.Any<string>());
        }
    }
}
