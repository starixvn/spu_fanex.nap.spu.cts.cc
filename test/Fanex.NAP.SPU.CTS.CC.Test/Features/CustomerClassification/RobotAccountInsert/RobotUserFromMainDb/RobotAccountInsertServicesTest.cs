namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotAccountInsertServicesTest
    {
        private readonly IRobotTWInsertDataAccess robotAccountInsertDataAccess;
        private readonly IRobotTWInsertServices robotTWInsertServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IDataLogTrackingServices dataLogTrackingServices;
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ISentryClient sentryClient;

        public RobotAccountInsertServicesTest()
        {
            robotAccountInsertDataAccess = Substitute.For<IRobotTWInsertDataAccess>();
            systemParameterServices = Substitute.For<ISystemParameterServices>();
            problemAccountServices = Substitute.For<IProblemAccountServices>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            dataLogTrackingServices = Substitute.For<IDataLogTrackingServices>();
            sentryClient = Substitute.For<ISentryClient>();
            baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            robotTWInsertServices = new RobotTWInsertServices(
                  robotAccountInsertDataAccess
                , systemParameterServices
                , custClassToMainDBService
                , dataLogTrackingServices
                , baseDataAccess
                , problemAccountDataAccess
                , sentryClient);
        }

        [Fact]
        public void GetLastTimespan_WhenSuccess()
        {
            // Act
            var result = robotTWInsertServices.GetLastTimespan();

            // Assert
            Assert.IsAssignableFrom<IRobotTWInsertServices>(result);
        }

        [Fact]
        public void GetRobotUser_WhenSuccess()
        {
            // Act
            var result = robotTWInsertServices.GetTWRobotUser();

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
            var result = robotTWInsertServices.InsertTWRobotPackage(1, null);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void InsertTWRobotPackage_WhenSuccess_ReturnTrue()
        {
            // Arrange
            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = 1,
                ServiceName = "robotAIClassServiceName",
                IsWriteLog = false,
                IsSyncAFCData = false,
            };

            long expectedLastTimespan = 1;

            var expectedRobotInfo = new TWRobotUserViewEntity
            {
                MaxTimestamp = 1,
                RobotUsers = Enumerable.Empty<TWRobotUserEntity>()
            };


            // Act
            systemParameterServices.GetLastScannedId(CTS_SystemParameters.RobotAccountLastScannedTime).Returns(expectedLastTimespan);

            robotTWInsertServices.GetLastTimespan();

            robotAccountInsertDataAccess.GetTWRobotUsers(expectedLastTimespan).Returns(expectedRobotInfo);

            robotTWInsertServices.GetTWRobotUser();

            var result = robotTWInsertServices.InsertTWRobotPackage(1, paras);

            // Assert
            result.Should().BeTrue();
        }

        private InsertRobotEntity CreateSampleRobotModel()
        {
            var rd = new Random();
            var data = new InsertRobotEntity
            {
                CustID = 0,
                IsFromTW = false,
                IsFromCTS = false,
                IsDirect = false,
                IsFromAI = false,
                IsFromTVS = false,
                RobotCounter = 0,
                CreatedBy = StarixITId,
                Remark = null,
                BetCount = null,
                TVSRequestID = null,
                ActiveDays = null,
                TurnoverRM = null,
                WinlossRM = null,
                WinlossStatus = null
            };

            return data;
        }
    }
}
