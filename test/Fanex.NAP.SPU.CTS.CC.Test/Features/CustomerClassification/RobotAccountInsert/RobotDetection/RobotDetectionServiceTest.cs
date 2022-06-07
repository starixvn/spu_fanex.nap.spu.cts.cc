namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotDetectionServiceTest
    {
        private readonly IRobotDetectionDataAccess robotDetectionDataAccess;
        private readonly IRobotDetectionService robotDetectionService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;        

        public RobotDetectionServiceTest()
        {
            robotDetectionDataAccess = Substitute.For<IRobotDetectionDataAccess>();           
            systemParameterServices = Substitute.For<ISystemParameterServices>();
            problemAccountServices = Substitute.For<IProblemAccountServices>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            antiFraudCenterServices = Substitute.For<IAntiFraudCenterServices>();
            robotDetectionService = new RobotDetectionService(robotDetectionDataAccess
                , systemParameterServices
                , custClassToMainDBService
                , problemAccountServices
                , antiFraudCenterServices);
        }

        [Fact]
        public void GetRobotDetections_WhenSuccess()
        {
            var result = robotDetectionService.GetRobotDetections(SystemParameters.RobotDetectionLastModifiedDate);

            // Assert
            Assert.IsAssignableFrom<IRobotDetectionService>(result);
        }

        [Fact]
        public void InsertProblemAccount_WhenSuccess_ReturnData()
        {
            // Arrange
            var robotDetections = new List<RobotDetectionEntity> { new RobotDetectionEntity { CustID = 11, LastModifiedDate = DateTime.Now } };
            var paras = new InsertRobotParaEntity();
            var expectedResult = new List<long>();

            // Act
            var result = robotDetectionService.InsertProblemAccount(robotDetections, paras);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Process_WhenSuccess_ReturnTrue()
        {
            // Arrange
            var paras = new InsertRobotParaEntity()
            {
                BatchExternalSize = 1,
                BatchInternalSize = 1,
                ServiceName = "robotDetectionServiceName",
                IsPushCCToSoon88 = true,
                Soon88FeedDataAPI = "",
                IsWriteLog = false,
                IsFromTW = false,
                IsFromCTS = false,
                IsDirect = true,
                IsFromAI = true
            };
            var expectedRobotDetectionLastModifiedDate = DateTime.Now;
            var expectedRobotDetections = new List<RobotDetectionEntity>()
            {
                new RobotDetectionEntity { CustID = 1, LastModifiedDate = DateTime.Now }
            };

            // Act
            systemParameterServices.GetLastScannedDateTime(SystemParameters.RobotDetectionLastModifiedDate).Returns(expectedRobotDetectionLastModifiedDate);
            robotDetectionDataAccess.GetRobotDetections(expectedRobotDetectionLastModifiedDate).Returns(expectedRobotDetections);
            robotDetectionService.GetRobotDetections(SystemParameters.RobotDetectionLastModifiedDate);
            
            var result = robotDetectionService.Process(paras);

            // Assert
            result.Should().BeTrue();
        }
    }
}
