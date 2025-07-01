using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification
{
    public class MatchMonitorClassificationTest
    {
        private readonly IBaseCustomerClassificationDataAccess _baseDataAccess;
        private readonly IProblemAccountDataAccess _problemAccountDataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly IMatchMonitorClassificationDataAccess _matchDataAccess;

        private readonly IMatchMonitorClassificationService _service;

        public MatchMonitorClassificationTest()
        {
            _baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            _problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _matchDataAccess = Substitute.For<IMatchMonitorClassificationDataAccess>();

            _service = new MatchMonitorClassificationService(
                            _baseDataAccess,
                            _problemAccountDataAccess,
                            _custClassToMainDBService,
                            _matchDataAccess);
        }

        #region GetMatchMonitor
        [Fact]
        public void GetMatchMonitor_WhenSuccess()
        {
            // Arrange


            // Act
            _matchDataAccess.GetMatchMonitor().Returns(new List<int>());

            var result = _service.GetMatchMonitor();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMatchMonitor_ThrowException()
        {
            // Arrange


            // Act
            _matchDataAccess.GetMatchMonitor().Throws<Exception>();

            Action action = () => _service.GetMatchMonitor();
            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region GetMatchInfosByIDs
        [Fact]
        public void GetMatchInfosByIDs_WhenSuccess()
        {
            // Arrange


            // Act
            _matchDataAccess.GetMatchInfosByIDs(Arg.Any<string>()).Returns(new List<MatchesInfosEntity>());

            var result = _service.GetMatchInfosByIDs("TEST");

            // Assert
            _matchDataAccess.Received(1).GetMatchInfosByIDs(Arg.Any<string>());
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMatchInfosByIDs_ReturnEmpty()
        {
            // Arrange


            // Act
            var result = _service.GetMatchInfosByIDs(string.Empty);

            // Assert
            _matchDataAccess.Received(0).GetMatchInfosByIDs(Arg.Any<string>());
            Assert.Empty(result);
        }

        [Fact]
        public void GetMatchInfosByIDs_ThrowException()
        {
            // Arrange


            // Act
            _matchDataAccess.GetMatchInfosByIDs(Arg.Any<string>()).Throws<Exception>();

            Action action = () => _service.GetMatchInfosByIDs("TEST");

            // Assert
            _matchDataAccess.Received(0).GetMatchInfosByIDs(Arg.Any<string>());
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateStatusMatchMonitor
        [Fact]
        public void UpdateStatusMatchMonitor_WhenSuccess()
        {
            // Arrange

            // Act
            _matchDataAccess.UpdateStatusMatchMonitor(Arg.Any<IEnumerable<MatchStatusModel>>());

            _service.UpdateStatusMatchMonitor(new List<MatchStatusModel>());
            // Assert
            _matchDataAccess.Received(1).UpdateStatusMatchMonitor(Arg.Any<IEnumerable<MatchStatusModel>>());
        }

        [Fact]
        public void UpdateStatusMatchMonitor_ThrowException()
        {
            // Arrange

            // Act
            _matchDataAccess.When(x => x.UpdateStatusMatchMonitor(Arg.Any<IEnumerable<MatchStatusModel>>())).Throw<Exception>();

            Action action = () => _service.UpdateStatusMatchMonitor(new List<MatchStatusModel>());
            // Assert
            _matchDataAccess.Received(0).UpdateStatusMatchMonitor(Arg.Any<IEnumerable<MatchStatusModel>>());
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region ClassifyMatchMonitor
        [Fact]
        public void ClassifyMatchMonitor_WhenSuccess()
        {
            // Arrange

            // Act
            _matchDataAccess.ClassifyMatchMonitor().Returns(new ClassifyGBMatchMonitorModel());

            var result = _service.ClassifyMatchMonitor();

            // Assert
            _matchDataAccess.Received(1).ClassifyMatchMonitor();
            Assert.NotNull(result);
        }

        [Fact]
        public void ClassifyMatchMonitor_ThrowException()
        {
            // Arrange

            // Act
            _matchDataAccess.ClassifyMatchMonitor().Throws<Exception>();

            Action action = () => _service.ClassifyMatchMonitor();

            // Assert
            _matchDataAccess.Received(0).ClassifyMatchMonitor();
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region ClassifyMatchMonitorCompleted
        [Fact]
        public void ClassifyMatchMonitorCompleted_WhenSuccess()
        {
            // Arrange
            var para = new ClassifyGBMatchMonitorModel
            {
                MatchID = 123,
                MMDetailIDList = "Test"
            };

            // Act
            _matchDataAccess.ClassifyMatchMonitorCompleted(Arg.Any<int>(), Arg.Any<string>());

            _service.ClassifyMatchMonitorCompleted(para);

            // Assert
            _matchDataAccess.Received(1).ClassifyMatchMonitorCompleted(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void ClassifyMatchMonitorCompleted_NoAction()
        {
            // Arrange
            var para = new ClassifyGBMatchMonitorModel();

            // Act
            _matchDataAccess.ClassifyMatchMonitorCompleted(Arg.Any<int>(), Arg.Any<string>());

            _service.ClassifyMatchMonitorCompleted(para);

            // Assert
            _matchDataAccess.Received(0).ClassifyMatchMonitorCompleted(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void ClassifyMatchMonitorCompleted_ThrowException()
        {
            // Arrange
            var para = new ClassifyGBMatchMonitorModel
            {
                MatchID = 123,
                MMDetailIDList = "Test"
            };

            // Act
            _matchDataAccess.When(x => x.ClassifyMatchMonitorCompleted(Arg.Any<int>(), Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.ClassifyMatchMonitorCompleted(para);

            // Assert
            _matchDataAccess.Received(0).ClassifyMatchMonitorCompleted(Arg.Any<int>(), Arg.Any<string>());
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertProblemAccount
        [Fact]
        public void InsertProblemAccount_WhenSuccess()
        {
            // Arrange
            var pas = new List<InsertProblemAccountEntity>();
            var paras = new PushDataToExternalEntity();

            // Act
            _custClassToMainDBService.PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), paras).Returns(true);

            var result = _service.InsertProblemAccount(pas, paras);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void InsertProblemAccount_ThrowException()
        {
            // Arrange
            var pas = new List<InsertProblemAccountEntity>();
            var paras = new PushDataToExternalEntity();

            // Act
            _custClassToMainDBService.PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), paras).Throws<Exception>();

            Action action = () => _service.InsertProblemAccount(pas, paras);

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion
    }
}
