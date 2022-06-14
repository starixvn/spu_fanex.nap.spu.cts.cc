namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;

    public class RobotAIClassificationInsertServicesTest
    {
        private readonly IRobotAIClassificationInsertDataAccess robotAIClassificationInsertDataAccess;
        private readonly IRobotAIClassificationInsertServices robotAIClassificationInsertServices;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;

        public RobotAIClassificationInsertServicesTest()
        {
            robotAIClassificationInsertDataAccess = Substitute.For<IRobotAIClassificationInsertDataAccess>();
            systemParameterServices = Substitute.For<ISystemParameterServices>();
            problemAccountServices = Substitute.For<IProblemAccountServices>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            antiFraudCenterServices = Substitute.For<IAntiFraudCenterServices>();
            robotAIClassificationInsertServices = new RobotAIClassificationInsertServices(robotAIClassificationInsertDataAccess
                , systemParameterServices
                , custClassToMainDBService
                , problemAccountServices
                , antiFraudCenterServices);
        }

        [Fact]
        public void GetRobotUserFromAIClassification_WhenSuccess()
        {           
            var result = robotAIClassificationInsertServices.GetRobotUserFromAIClassification();            

            // Assert
            Assert.IsAssignableFrom<IRobotAIClassificationInsertServices>(result);
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

            var expectedRobotAIEntity = new List<RobotAIEntity> { new RobotAIEntity { CustID = 1, ReasonID = 1 } };

            var expectedRobotAIViewEntity = new RobotAIViewEntity()
            {
                LastScannedId = 100,
                RobotAIClassification = expectedRobotAIEntity
            };

            var expectedCustomerInfoInsertRobot = new List<CustInfoEntity> { new CustInfoEntity { CustID = 1, CTSCustID = 1 } };

            var expectedInsertRobotEntity = new List<InsertRobotEntity> { new InsertRobotEntity { CustID = 1 } };

            // Act
            robotAIClassificationInsertDataAccess.GetRobotUsersFromAIClassification().Returns(expectedRobotAIViewEntity);

            robotAIClassificationInsertServices.GetRobotUserFromAIClassification();            

            problemAccountServices.InsertRobotAccount(expectedInsertRobotEntity).Returns(expectedCustomerInfoInsertRobot);
            
            var result = robotAIClassificationInsertServices.Process(paras);

            // Assert
            result.Should().BeTrue();
        }

    }
}
