namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class RealtimeCheckChangeClassificationServiceTest
    {
        private readonly IRealtimeCheckChangeClassificationDataAccess realtimeCheckChangeDataAccess;
        private readonly IRealtimeCheckChangeClassificationService realtimeCheckChangeService;

        public RealtimeCheckChangeClassificationServiceTest()
        {
            realtimeCheckChangeDataAccess = Substitute.For<IRealtimeCheckChangeClassificationDataAccess>();
            realtimeCheckChangeService = new RealtimeCheckChangeClassificationService(realtimeCheckChangeDataAccess, null);
        }

        [Fact]
        public void CheckChange_WhenSuccess()
        {
            // Arrange
            var checkChangeModel = new RealtimeCheckChangeModel();

            // Act
            realtimeCheckChangeDataAccess.CheckChange().Returns(checkChangeModel);

            var result = realtimeCheckChangeService.CheckChange();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(checkChangeModel);
        }

        [Fact]
        public void UpdateLastTransDate_WhenSuccess()
        {
            // Arrange
            var custInfos = new List<RealtimeCheckChangeEntities>();
            var custInfosJson = JsonConvert.SerializeObject(custInfos);

            // Act            
            realtimeCheckChangeService.UpdateLastTransDate(custInfos);

            // Assert
            realtimeCheckChangeDataAccess.Received(1).UpdateLastTransDate(custInfosJson);            
        }

        [Fact]
        public void UpdateLastTransDate_WhenDidNotUpdate()
        {
            // Arrange
            var custInfos = new List<RealtimeCheckChangeEntities>();
            var custInfosJson = JsonConvert.SerializeObject(custInfos);

            // Act

            // Assert
            realtimeCheckChangeDataAccess.DidNotReceive().UpdateLastTransDate(custInfosJson);
        }

        [Fact]
        public void UpdateNextScanCheckChange_WhenSuccess()
        {
            // Arrange
            var nextScanned = new RealtimeCheckChangeModel();

            // Act
            realtimeCheckChangeService.UpdateNextScanCheckChange(nextScanned);

            // Assert
            realtimeCheckChangeDataAccess.Received(1).UpdateNextScanCheckChange(nextScanned);
        }

        [Fact]
        public void UpdateNextScanCheckChange_WhenDidNotUpdate()
        {
            // Arrange
            var nextScanned = new RealtimeCheckChangeModel();

            // Act            

            // Assert
            realtimeCheckChangeDataAccess.DidNotReceive().UpdateNextScanCheckChange(nextScanned);
        }

    }
}
