using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring.Models;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.JobMonitoring
{
    public class IncorrectDataMonitoringServiceTest
    {
        private readonly IIncorrectDataMonitoringDataAccess _dataAccess;
        private readonly IIncorrectDataMonitoringService _service;

        public IncorrectDataMonitoringServiceTest()
        {
            _dataAccess = Substitute.For<IIncorrectDataMonitoringDataAccess>();
            _service = new IncorrectDataMonitoringService(_dataAccess);
        }

        [Fact]
        public void GetIncorrectDataMonitoring_WhenCalled_ShouldCallDataAccess()
        {
            // Arrange
            var expectedResult = new GetNotificationMonitoringEntity();
            _dataAccess.GetIncorrectDataMonitoring().Returns(expectedResult);

            // Act
            var result = _service.GetIncorrectDataMonitoring();

            // Assert
            _dataAccess.Received(1).GetIncorrectDataMonitoring();
            Assert.Same(expectedResult, result);
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithInsertMonitoringEntityList_WhenListIsEmpty_ShouldNotCallDataAccess()
        {
            // Arrange
            var emptyList = new List<InsertMonitoringEntity>();
            
            // Act
            _service.InsertIncorrectDataMonitoring(emptyList, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 100);

            // Assert
            _dataAccess.DidNotReceive().InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithInsertMonitoringEntityList_WhenListIsNull_ShouldNotCallDataAccess()
        {
            // Arrange
            List<InsertMonitoringEntity> nullList = null;
            
            // Act
            _service.InsertIncorrectDataMonitoring(nullList, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 100);

            // Assert
            _dataAccess.DidNotReceive().InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithInsertMonitoringEntityList_WhenListHasItems_ShouldCallDataAccess()
        {
            // Arrange
            var list = new List<InsertMonitoringEntity>
            {
                new InsertMonitoringEntity { CustID = 1 },
                new InsertMonitoringEntity { CustID = 2 }
            };
            
            // Act
            _service.InsertIncorrectDataMonitoring(list, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 100);

            // Assert
            _dataAccess.Received(1).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Is<JobMonitor_IssueType>(x => x == JobMonitor_IssueType.Classification_ExcessiveGeneralPool));
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithInsertMonitoringEntityList_WhenBatchSizeIsSmaller_ShouldMakeMultipleCalls()
        {
            // Arrange
            var list = new List<InsertMonitoringEntity>
            {
                new InsertMonitoringEntity { CustID = 1 },
                new InsertMonitoringEntity { CustID = 2 },
                new InsertMonitoringEntity { CustID = 3 },
                new InsertMonitoringEntity { CustID = 4 }
            };
            
            // Act
            _service.InsertIncorrectDataMonitoring(list, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 2);

            // Assert
            _dataAccess.Received(2).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Is<JobMonitor_IssueType>(x => x == JobMonitor_IssueType.Classification_ExcessiveGeneralPool));
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithLongList_WhenListIsEmpty_ShouldNotCallDataAccess()
        {
            // Arrange
            var emptyList = new List<long>();
            
            // Act
            _service.InsertIncorrectDataMonitoring(emptyList, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 100);

            // Assert
            _dataAccess.DidNotReceive().InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithLongList_WhenListIsNull_ShouldNotCallDataAccess()
        {
            // Arrange
            List<long> nullList = null;
            
            // Act
            _service.InsertIncorrectDataMonitoring(nullList, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 100);

            // Assert
            _dataAccess.DidNotReceive().InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithLongList_WhenListHasItems_ShouldCallDataAccess()
        {
            // Arrange
            var list = new List<long> { 1, 2 };
            
            // Act
            _service.InsertIncorrectDataMonitoring(list, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 100);

            // Assert
            _dataAccess.Received(1).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Is<JobMonitor_IssueType>(x => x == JobMonitor_IssueType.Classification_ExcessiveGeneralPool));
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithLongList_WhenBatchSizeIsSmaller_ShouldMakeMultipleCalls()
        {
            // Arrange
            var list = new List<long> { 1, 2, 3, 4 };
            
            // Act
            _service.InsertIncorrectDataMonitoring(list, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 2);

            // Assert
            _dataAccess.Received(2).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Is<JobMonitor_IssueType>(x => x == JobMonitor_IssueType.Classification_ExcessiveGeneralPool));
        }

        [Fact]
        public void InsertIncorrectDataMonitoring_WithLongList_ShouldConvertToInsertMonitoringEntities()
        {
            // Arrange
            var list = new List<long> { 1, 2 };
            
            // Act
            _service.InsertIncorrectDataMonitoring(list, JobMonitor_IssueType.Classification_ExcessiveGeneralPool, 100);

            // Assert
            _dataAccess.Received(1).InsertIncorrectDataMonitoring(
                Arg.Is<string>(s => s.Contains("CustID") && s.Contains("1") && s.Contains("2")),
                Arg.Is<JobMonitor_IssueType>(x => x == JobMonitor_IssueType.Classification_ExcessiveGeneralPool));
        }
    }
} 