namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotAccountInsertServicesTest
    {
        private readonly IRobotAccountInsertDataAccess robotAccountInsertDataAccess;
        private readonly IRobotAccountInsertServices robotAccountInsertServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;

        public RobotAccountInsertServicesTest()
        {
            robotAccountInsertDataAccess = Substitute.For<IRobotAccountInsertDataAccess>();
            systemParameterServices = Substitute.For<ISystemParameterServices>();
            problemAccountServices = Substitute.For<IProblemAccountServices>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            antiFraudCenterServices = Substitute.For<IAntiFraudCenterServices>();
            robotAccountInsertServices = new RobotAccountInsertServices(robotAccountInsertDataAccess
                , systemParameterServices
                , custClassToMainDBService
                , problemAccountServices
                , antiFraudCenterServices);
        }

        [Fact]
        public void GetLastTimespan_WhenSuccess()
        {
            // Act
            var result = robotAccountInsertServices.GetLastTimespan();

            // Assert
            Assert.IsAssignableFrom<IRobotAccountInsertServices>(result);
        }

        [Fact]
        public void GetRobotUser_WhenSuccess()
        {
            // Act
            var result = robotAccountInsertServices.GetRobotUser();

            // Assert
            Assert.IsAssignableFrom<IRobotAccountInsertServices>(result);
        }

        [Fact]
        public void Process_WhenSuccess_ReturnTrue()
        {
            // Arrange
            var paras = new InsertRobotParaEntity {
                BatchExternalSize = 1,
                BatchInternalSize = 1,
                ServiceName = "robotAccountServiceName",
                Soon88FeedDataAPI = "",
                IsPushCCToSoon88 = true,
                IsWriteLog = false,
                IsSyncAFCData = false,
                AFCInsertAccountURL = "",
                IsFromTW = true,
                IsFromCTS = false,
                IsDirect = true,
                IsFromAI = false
            };

            long expectedLastTimespan = 1;

            var expectedRobotInfo = new RobotUserViewEntity
            {
                MaxTimestamp = 1,
                RobotUsers = new List<RobotUserEntity> { new RobotUserEntity { CustId = 1, Counter = 1 } }
            };


            // Act
            systemParameterServices.GetLastScannedId(SystemParameters.RobotAccountLastScannedTime).Returns(expectedLastTimespan);

            robotAccountInsertServices.GetLastTimespan();

            robotAccountInsertDataAccess.GetRobotUsersFromMainDb(expectedLastTimespan).Returns(expectedRobotInfo);

            robotAccountInsertServices.GetRobotUser();

            var result = robotAccountInsertServices.Process(paras);

            // Assert
            result.Should().BeTrue();
        }
    }
}
