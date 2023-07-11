namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
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
        private readonly IRobotAIDetectionDataAccess robotDetectionDataAccess;
        private readonly IRobotAIDetectionService robotDetectionService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;

        public RobotDetectionServiceTest()
        {
            robotDetectionDataAccess = Substitute.For<IRobotAIDetectionDataAccess>();           
            systemParameterServices = Substitute.For<ISystemParameterServices>();
            problemAccountServices = Substitute.For<IProblemAccountServices>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            robotDetectionService = new RobotAIDetectionService(robotDetectionDataAccess
                , systemParameterServices
                , custClassToMainDBService
                , problemAccountServices);
        }

        [Fact]
        public void GetRobotDetections_WhenSuccess()
        {
            var result = robotDetectionService.GetRobotDetections(SystemParameters.RobotDetection_LastID, SystemParameters.TWRobotImperva_LastID);

            // Assert
            Assert.IsAssignableFrom<IRobotAIDetectionService>(result);
        }

        [Fact]
        public void InsertProblemAccount_WhenSuccess_ReturnData()
        {
            // Arrange
            var robotDetections = new List<RobotDetectionEntity> { new RobotDetectionEntity { CustID = 0, LastModifiedDate = DateTime.Now } };
            var paras = new InsertRobotParaEntity();
            var expectedResult = new List<long>();

            // Act
            var result = robotDetectionService.InsertRobotAIImperva(robotDetections, paras);

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
                BatchInternalSize = 1,
                IsFromTW = false,
                IsFromCTS = false,
                IsDirect = true,
                IsFromAI = true,
                IsFromTVS = false,
                PushDataExternal = new PushDataToExternalEntity
                {
                    ExternalBatchSize = 1,
                    ServiceName = "robotDetectionServiceName",
                    IsPushCCToSoon88 = true,
                    IsWriteLog = false,
                    IsSyncAFCData = false,
                }
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
            systemParameterServices.GetLastScannedId(SystemParameters.RobotDetection_LastID).Returns(expectedRobotAIID);
          
            robotDetectionDataAccess.GetRobotDetections(expectedRobotAIID, expectedRobotImpervaID).Returns((expectedRobotAI, expectedRobotImperva));
            
            var result = robotDetectionService.InsertRobotAIImpervaPackage(paras);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void MappingRobotCategory_WhenSuccess_ReturnData()
        {
            // Arrange
            var robotDetections = new List<RobotDetectionEntity> { new RobotDetectionEntity { CustID = 0, LastModifiedDate = DateTime.Now } };
            var paras = new InsertRobotParaEntity();
            var expectedResult = new List<InsertRobotEntity>()
            {
                CreateSampleRobotModel()
            };

            // Act
            var result = robotDetectionService.MappingRobotCategory(robotDetections, paras);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
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
                RobotCounter = null,
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
