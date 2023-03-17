namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.TWRobotImperva
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using Xunit;

    public class TWRobotImpervaServiceTest
    {
        private readonly ITWRobotImpervaDataAccess robotImpervaDataAccess;
        private readonly ISystemParameterDataAccess sysDataAccess;
        private readonly ITWRobotImpervaService robotImpervaService;

        public TWRobotImpervaServiceTest()
        {
            robotImpervaDataAccess = Substitute.For<ITWRobotImpervaDataAccess>();
            sysDataAccess = Substitute.For<ISystemParameterDataAccess>();
            
            robotImpervaService = new TWRobotImpervaService(robotImpervaDataAccess, sysDataAccess);
        }

        [Fact]
        public void GetLastScannedID_WhenSuccess()
        {            
            // Act
            var result = robotImpervaService.GetLastScannedID(Shares.Constants.SystemParameters.TWRobotImperva_LastLogID);

            // Assert
            Assert.IsAssignableFrom<long>(result);
        }

        [Fact]
        public void GetTWRobotImperva_WhenSuccess_ReturnRobot()
        {
            // Arrange
            var logID = 1;
            var robot = new TWRobotImpervaGetModel();

            // Act
            robotImpervaDataAccess.GetTWRobotImperva(logID).Returns(robot);
            var result = robotImpervaService.GetTWRobotImperva(logID);

            // Assert      
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(robot);
        }
    }
}
