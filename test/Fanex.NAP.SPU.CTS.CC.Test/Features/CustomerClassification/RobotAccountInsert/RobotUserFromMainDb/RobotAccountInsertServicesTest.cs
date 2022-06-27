namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
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
        private readonly IRobotTWInsertDataAccess robotAccountInsertDataAccess;
        private readonly IRobotTWInsertServices robotAccountInsertServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IDataLogTrackingServices dataLogTrackingServices;

        public RobotAccountInsertServicesTest()
        {
            robotAccountInsertDataAccess = Substitute.For<IRobotTWInsertDataAccess>();
            systemParameterServices = Substitute.For<ISystemParameterServices>();
            problemAccountServices = Substitute.For<IProblemAccountServices>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            dataLogTrackingServices = Substitute.For<IDataLogTrackingServices>();
            robotAccountInsertServices = new RobotTWInsertServices(robotAccountInsertDataAccess
                , systemParameterServices
                , custClassToMainDBService
                , problemAccountServices
                , dataLogTrackingServices);
        }

        [Fact]
        public void GetLastTimespan_WhenSuccess()
        {
            // Act
            var result = robotAccountInsertServices.GetLastTimespan();

            // Assert
            Assert.IsAssignableFrom<IRobotTWInsertServices>(result);
        }

        [Fact]
        public void GetRobotUser_WhenSuccess()
        {
            // Act
            var result = robotAccountInsertServices.GetTWRobotUser();

            // Assert
            Assert.IsAssignableFrom<IRobotTWInsertServices>(result);
        }

        [Fact]
        public void Process_WhenSuccess_ReturnTrue()
        {
            // Arrange
            var paras = new InsertRobotParaEntity 
            {
                BatchInternalSize = 1,
                IsFromTW = true,
                IsFromCTS = false,
                IsDirect = true,
                IsFromAI = false,
                IsFromTVS = false,
                PushDataExternal = new PushDataToExternalEntity
                {
                    ExternalBatchSize = 1,
                    ServiceName = "robotAIClassServiceName",
                    IsPushCCToSoon88 = true,
                    IsWriteLog = false,
                    IsSyncAFCData = false,
                }
            };

            long expectedLastTimespan = 1;

            var expectedRobotInfo = new TWRobotUserViewEntity
            {
                MaxTimestamp = 1,
                RobotUsers = new List<TWRobotUserEntity> { new TWRobotUserEntity { CustId = 1, Counter = 1 } }
            };


            // Act
            systemParameterServices.GetLastScannedId(SystemParameters.RobotAccountLastScannedTime).Returns(expectedLastTimespan);

            robotAccountInsertServices.GetLastTimespan();

            robotAccountInsertDataAccess.GetTWRobotUsers(expectedLastTimespan).Returns(expectedRobotInfo);

            robotAccountInsertServices.GetTWRobotUser();

            var result = robotAccountInsertServices.InsertTWRobotPackage(paras);

            // Assert
            result.Should().BeTrue();
        }
    }
}
