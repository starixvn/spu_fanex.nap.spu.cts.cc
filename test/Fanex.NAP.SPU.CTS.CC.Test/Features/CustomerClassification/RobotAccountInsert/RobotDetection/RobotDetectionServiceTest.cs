namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotDetectionServiceTest
    {
        private readonly IRobotAIDetectionDataAccess robotDetectionDataAccess;
        private readonly IRobotAIDetectionService robotDetectionService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly ISentryClient sentryClient;

        public RobotDetectionServiceTest()
        {
            robotDetectionDataAccess = Substitute.For<IRobotAIDetectionDataAccess>();           
            systemParameterServices = Substitute.For<ISystemParameterServices>();
            problemAccountServices = Substitute.For<IProblemAccountServices>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            sentryClient = Substitute.For<ISentryClient>();
            baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            robotDetectionService = new RobotAIDetectionService(robotDetectionDataAccess
                , systemParameterServices
                , custClassToMainDBService
                , baseDataAccess
                , problemAccountDataAccess
                , sentryClient);
        }

        [Fact]
        public void GetRobotDetections_WhenSuccess()
        {
            var result = robotDetectionService.GetRobotDetections(CTS_SystemParameters.RobotDetection_LastID);

            // Assert
            Assert.IsAssignableFrom<IRobotAIDetectionService>(result);
        }

        [Fact]
        public void InsertProblemAccount_WhenSuccess_ReturnData()
        {
            // Arrange
            var robotDetections = new List<RobotDetectionEntity> { new RobotDetectionEntity { CustID = 0, LastModifiedDate = DateTime.Now } };
            var expectedResult = new List<long>();
            var config = new PushDataToExternalEntity { ExternalBatchSize = 10 };

            // Act
            var result = robotDetectionService.InsertAIRobotPackage(100, config);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Process_WhenSuccess_ReturnTrue()
        {
            // Arrange
            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = 1,
                ServiceName = "robotDetectionServiceName",
                IsWriteLog = false,
                IsSyncAFCData = false,
            };

            var expectedRobotAIID = 1;
            var expectedRobotImpervaID = 2;
            var expectedRobotAI = new List<RobotAIEntity>()
            {
                new RobotAIEntity { CustID = 1, CategoryID = 1 , RobotDetectionID = 1, LastModifiedDate = DateTime.Now },
                new RobotAIEntity { CustID = 2, CategoryID = 1 , RobotDetectionID = 2, LastModifiedDate = DateTime.Now}
            };
            var expectedRobotImperva = new List<RobotImpervaEntity>()
            { 
                new RobotImpervaEntity { CustID = 1, LastCreateTime = DateTime.Now, RobotImpervaLogID = 1 },
                new RobotImpervaEntity { CustID = 2, LastCreateTime = DateTime.Now, RobotImpervaLogID = 2 }
            };

            // Act
            systemParameterServices.GetLastScannedId(CTS_SystemParameters.RobotDetection_LastID).Returns(expectedRobotAIID);
          
            robotDetectionDataAccess.GetRobotDetections(expectedRobotAIID).Returns((expectedRobotAI));
            
            var result = robotDetectionService.InsertAIRobotPackage(1, paras);

            // Assert
            result.Should().BeTrue();
        }
      
    }
}
