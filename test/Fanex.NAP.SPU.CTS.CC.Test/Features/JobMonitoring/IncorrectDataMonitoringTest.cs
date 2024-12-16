using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using Fanex.NAP.SPU.Notification.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.IncorrectDataMonitoringConsts;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.JobMonitoring
{
    public class IncorrectDataMonitoringTest
    {
        private readonly IIncorrectDataMonitoringDataAccess _dataAccess;

        private readonly IIncorrectDataMonitoringService _service;

        public IncorrectDataMonitoringTest()
        {
            _dataAccess = Substitute.For<IIncorrectDataMonitoringDataAccess>();

            _service = new IncorrectDataMonitoringService(_dataAccess);
        }

        #region GetIncorrectDataMonitoring
        [Fact]
        public void GetIncorrectDataMonitoring_WhenSuccess()
        {
            // Arrange

            // Act
            _dataAccess.GetIncorrectDataMonitoring().Returns(new List<GetMonitoringEntity>());

            var result = _service.GetIncorrectDataMonitoring();
            // Assert
            _dataAccess.Received(1).GetIncorrectDataMonitoring();

            Assert.NotNull(result);
        }

        [Fact]

        public void GetIncorrectDataMonitoring_ThrowException()
        {
            // Arrange

            // Act
            _dataAccess.GetIncorrectDataMonitoring().Throws<Exception>();

            Action action = () => _service.GetIncorrectDataMonitoring();
            // Assert
            _dataAccess.Received(0).GetIncorrectDataMonitoring();

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertIncorrectDataMonitoring
        [Fact]
        public void InsertIncorrectDataMonitoringV1_WhenSuccess()
        {
            // Arrange
            var data = new List<InsertMonitoringEntity>() {
                new InsertMonitoringEntity(),
                new InsertMonitoringEntity(),
                new InsertMonitoringEntity()
            };

            // Act
            _dataAccess.InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());

            _service.InsertIncorrectDataMonitoring(data, JobMonitor_IssueType.Classification_MissingTVSProblemAccount, 1);

            // Assert
            _dataAccess.Received(3).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoringV1_WhenSuccess_NoParamValue()
        {
            // Arrange

            // Act
            _service.InsertIncorrectDataMonitoring(Enumerable.Empty<InsertMonitoringEntity>(), JobMonitor_IssueType.Classification_MissingTVSProblemAccount, 1);

            // Assert
            _dataAccess.Received(0).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoringV1_ThrowException()
        {
            // Arrange
            var data = new List<InsertMonitoringEntity>() {
                new InsertMonitoringEntity(),
                new InsertMonitoringEntity(),
                new InsertMonitoringEntity()
            };

            // Act
            _dataAccess.When(x => x.InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>())).Throw<Exception>();

            Action action = () => _service.InsertIncorrectDataMonitoring(data, JobMonitor_IssueType.Classification_MissingTVSProblemAccount, 1);

            // Assert
            _dataAccess.Received(0).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertIncorrectDataMonitoring
        [Fact]
        public void InsertIncorrectDataMonitoringV2_WhenSuccess()
        {
            // Arrange
            var data = new List<long>() { 1, 2, 3, 4, 5 };

            // Act
            _dataAccess.InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());

            _service.InsertIncorrectDataMonitoring(data, JobMonitor_IssueType.Classification_MissingTVSProblemAccount, 2);

            // Assert
            _dataAccess.Received(3).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoringV2_WhenSuccess_NoParamValue()
        {
            // Arrange

            // Act
            _service.InsertIncorrectDataMonitoring(Enumerable.Empty<long>(), JobMonitor_IssueType.Classification_MissingTVSProblemAccount, 1);

            // Assert
            _dataAccess.Received(0).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());
        }

        [Fact]
        public void InsertIncorrectDataMonitoringV2_ThrowException()
        {
            // Arrange
            var data = new List<long>() { 1, 2, 3, 4, 5 };

            // Act
            _dataAccess.When(x => x.InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>())).Throw<Exception>();

            Action action = () => _service.InsertIncorrectDataMonitoring(data, JobMonitor_IssueType.Classification_MissingTVSProblemAccount, 1);

            // Assert
            _dataAccess.Received(0).InsertIncorrectDataMonitoring(Arg.Any<string>(), Arg.Any<JobMonitor_IssueType>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region GenerateMessage
        [Theory]
        [InlineData(JobMonitor_IssueType.NewCustomer_NullCategory)]
        [InlineData(JobMonitor_IssueType.Realtime_NullCategory)]
        [InlineData(JobMonitor_IssueType.NewCustomer_Missing)]
        [InlineData(JobMonitor_IssueType.NewCustomer_IncorrectCustInfo)]
        [InlineData(JobMonitor_IssueType.Realtime_NewPlaceBetInactiveCategory)]
        [InlineData(JobMonitor_IssueType.Classification_ExcessiveGeneralPool)]
        [InlineData(JobMonitor_IssueType.Classification_ExcessiveBySportPool)]
        [InlineData(JobMonitor_IssueType.DCSTransaction_LastLoginProcessStatus)]
        [InlineData(JobMonitor_IssueType.Classification_MissingTVSProblemAccount)]
        public void GenerateMessage_WhenSuccess(JobMonitor_IssueType issueTypeID)
        {
            // Arrange
            var generateMessageMethod = TestExtension.GetPrivateStaticMethod(typeof(IncorrectDataMonitoringService), PrivateMethodName.GenerateMessage);

            var data = new List<GetMonitoringEntity> { new GetMonitoringEntity() { IssueTypeID = (int)issueTypeID, CustID = 999 } };

            // Act
            var message = generateMessageMethod.Invoke(_service, new object[] { data, issueTypeID });

            // Assert
            Assert.NotNull(message);
        }
        #endregion
    }
}
