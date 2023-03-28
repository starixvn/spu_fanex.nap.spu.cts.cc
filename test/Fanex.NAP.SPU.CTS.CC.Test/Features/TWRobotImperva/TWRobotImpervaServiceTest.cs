namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.TWRobotImperva
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva;
    using Fanex.NAP.SPU.CTS.CC.Features.TWRobotImperva.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

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

        [Fact]
        public void InsertTWRobotImperva_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var robots = new List<TWRobotImpervaGetEntity>()
            {
                new TWRobotImpervaGetEntity
                {
                    logid = 1,
                    CustId = 1,
                    CreateTime  = new DateTime(2023, 3, 24),
                    Platform = 1
                },
                new TWRobotImpervaGetEntity
                {
                    logid = 2,
                    CustId = 2,
                    CreateTime  = new DateTime(2023, 3, 24),
                    Platform = 2
                }
            };

            // Act
            Action action = () => robotImpervaService.InsertTWRobotImperva(robots);

            // Assert
            action.Should().NotThrow<Exception>();
        }

        [Theory]
        [InlineData(SystemParameters.TWRobotImperva_LastLogID, 17)]
        public void UpdateLastScannedID_WhenSuccess(SystemParameters para, long maxLogID)
        {
            // Arrange
            sysDataAccess.UpdateTimingExecution(maxLogID.ToString(), para);
           
            // Act
            Action action = () => robotImpervaService.UpdateLastScannedID(para, maxLogID);

            // Assert
            action.Should().NotThrow<Exception>();
        }

    }
}
