namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;

    public class RobotAIClassificationInsertServicesTest
    {
        private readonly IRobotAIClassificationInsertDataAccess robotAIClassificationInsertDataAccess;
        private readonly IRobotAIClassificationInsertServices robotAIClassificationInsertServices;

        public RobotAIClassificationInsertServicesTest()
        {
            robotAIClassificationInsertDataAccess = Substitute.For<IRobotAIClassificationInsertDataAccess>();
            robotAIClassificationInsertServices = new RobotAIClassificationInsertServices(robotAIClassificationInsertDataAccess
               , null
               , null
               , null
               , null);
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
            var paras = new InsertRobotParaEntity();

            // Act
            var result = robotAIClassificationInsertServices.Process(paras);

            // Assert
            result.Should().BeTrue();
        }

    }
}
