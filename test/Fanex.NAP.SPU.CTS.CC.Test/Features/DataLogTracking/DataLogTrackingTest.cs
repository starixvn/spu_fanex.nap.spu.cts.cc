using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
using Fanex.NAP.SPU.CTS.Shares.Models;
using NSubstitute;
using Sentry;
using System;
using System.Collections.Generic;
using Xunit;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.DataLogTracking
{
    public class DataLogTrackingTest
    {
        private readonly IDataLogTrackingDataAccess _dataAccess;
        private readonly ISentryClient _sentryClient;

        private readonly IDataLogTrackingServices _service;

        public DataLogTrackingTest()
        {
            _dataAccess = Substitute.For<IDataLogTrackingDataAccess>();
            _sentryClient = Substitute.For<ISentryClient>();

            _service = new DataLogTrackingServices(_dataAccess, _sentryClient);
        }

        #region LogInputData
        [Fact]
        public void LogInputData_WhenSuccess()
        {
            // Arrange
            var content = new LogInputViewEntity { RobotUser = new List<TWRobotUserEntity> { new TWRobotUserEntity { CustId = 333, Counter = 666 } } };

            var dbResult = new DBResult();

            // Act
            _dataAccess.LogInputData(Arg.Any<IEnumerable<LogInputDataEntity>>(), FunctionId.ClassifyGBMatchMonitor).Returns(dbResult);

            _service.LogInputData(content, FunctionId.ClassifyGBMatchMonitor, "TEST");

            // Assert
            _dataAccess.Received(1).LogInputData(Arg.Any<IEnumerable<LogInputDataEntity>>(), Arg.Any<FunctionId>());
        }
        #endregion

        #region LogErrorData
        [Fact]
        public void LogErrorData_WhenSuccess()
        {
            // Arrange
            var exception = new Exception("TEST");

            // Act
            _dataAccess.LogErrorData(Arg.Any<LogErrorDataEntity>());

            _service.LogErrorData(exception, "TEST");

            // Assert
            _dataAccess.Received(1).LogErrorData(Arg.Any<LogErrorDataEntity>());
        }
        #endregion
    }
}
