namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class RobotDetectionServiceTest
    {
        private readonly IRobotDetectionDataAccess robotDetectionDataAccess;
        private readonly IRobotDetectionService robotDetectionService;    

        public RobotDetectionServiceTest()
        {
            robotDetectionDataAccess = Substitute.For<IRobotDetectionDataAccess>();
            robotDetectionService = new RobotDetectionService(robotDetectionDataAccess
               , null
               , null
               , null
               , null);
        }

        [Fact]
        public void GetRobotDetections_WhenSuccess()
        {
            var result = robotDetectionService.GetRobotDetections();

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
            var paras = new InsertRobotParaEntity();

            // Act
            var result = robotDetectionService.Process(paras);

            // Assert
            result.Should().BeTrue();
        }
    }
}
