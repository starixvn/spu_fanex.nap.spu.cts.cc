namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class MatchMonitorClassificationBySportServiceTest
    {
        private readonly IMatchMonitorClassificationBySportDataAccess _matchDataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly IProblemAccountBySportService _problemAccountBySportService;
        private readonly IMatchMonitorClassificationBySportService _service;

        public MatchMonitorClassificationBySportServiceTest()
        {
            _matchDataAccess = Substitute.For<IMatchMonitorClassificationBySportDataAccess>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _problemAccountBySportService = Substitute.For<IProblemAccountBySportService>();

            _service = new MatchMonitorClassificationBySportService(
                _custClassToMainDBService,
                _matchDataAccess,
                _problemAccountBySportService);
        }

        #region ClassifyMatchMonitor
        [Fact]
        public void ClassifyMatchMonitor_WhenSuccess()
        {
            // Arrange
            var expectedModel = new ClassifyGBMatchMonitorBySportModel
            {
                CustInfos = new List<ClassifyGBMMCustInfosBySportEntity>
                {
                    new ClassifyGBMMCustInfosBySportEntity { CustID = 1, MatchID = 100 }
                },
                MatchID = 100,
                MMDetailIDList = "1,2,3"
            };

            _matchDataAccess.ClassifyMatchMonitor(500).Returns(expectedModel);

            // Act
            var result = _service.ClassifyMatchMonitor(500);

            // Assert
            _matchDataAccess.Received(1).ClassifyMatchMonitor(500);
            Assert.NotNull(result);
            Assert.Equal(expectedModel.MatchID, result.MatchID);
            Assert.Equal(expectedModel.MMDetailIDList, result.MMDetailIDList);
            Assert.NotNull(result.CustInfos);
        }

        [Fact]
        public void ClassifyMatchMonitor_ThrowException()
        {
            // Arrange
            _matchDataAccess.ClassifyMatchMonitor(500).Throws<Exception>();

            // Act
            Action action = () => _service.ClassifyMatchMonitor(500);

            // Assert
            _matchDataAccess.Received(0).ClassifyMatchMonitor(500);
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region ClassifyMatchMonitorCompleted
        [Fact]
        public void ClassifyMatchMonitorCompleted_WhenSuccess()
        {
            // Arrange
            var model = new ClassifyGBMatchMonitorBySportModel
            {
                MatchID = 123,
                MMDetailIDList = "1,2,3"
            };

            // Act
            _service.ClassifyCompletedMatchMonitor(model);

            // Assert
            _matchDataAccess.Received(1).ClassifyCompletedMatchMonitor(123, "1,2,3");
        }

        [Fact]
        public void ClassifyMatchMonitorCompleted_NoAction_WhenMatchIDIsNull()
        {
            // Arrange
            var model = new ClassifyGBMatchMonitorBySportModel
            {
                MatchID = null,
                MMDetailIDList = "1,2,3"
            };

            // Act
            _service.ClassifyCompletedMatchMonitor(model);

            // Assert
            _matchDataAccess.Received(0).ClassifyCompletedMatchMonitor(Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public void ClassifyCompletedMatchMonitor_ThrowException()
        {
            // Arrange
            var model = new ClassifyGBMatchMonitorBySportModel
            {
                MatchID = 123,
                MMDetailIDList = "1,2,3"
            };

            _matchDataAccess.When(x => x.ClassifyCompletedMatchMonitor(Arg.Any<int>(), Arg.Any<string>())).Throw<Exception>();

            // Act
            Action action = () => _service.ClassifyCompletedMatchMonitor(model);

            // Assert
            _matchDataAccess.Received(0).ClassifyCompletedMatchMonitor(Arg.Any<int>(), Arg.Any<string>());
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertProblemAccount
        [Fact]
        public void InsertProblemAccount_WhenSuccess()
        {
            // Arrange
            var paBySports = new List<InsertProblemAccountBySportEntity>
            {
                new InsertProblemAccountBySportEntity
                {
                    CustID = 1,
                    CTSCustID = 100,
                    MatchID = 200,
                    SportGroup = 1
                }
            };

            var paras = new PushDataToExternalEntity
            {
                ServiceName = "TestService"
            };

            var expectedCustIDs = new List<InsertPABySportResultModel> { };
            _problemAccountBySportService.InsertProblemAccountBySport(Arg.Any<IEnumerable<InsertProblemAccountBySportEntity>>(), Arg.Any<CCInputFlow>(), BySport_FunctionId.InsertPACategory)
                .Returns(expectedCustIDs);

            // Act
            var result = _service.InsertProblemAccount(paBySports, paras);

            // Assert
            _problemAccountBySportService.Received(1).InsertProblemAccountBySport(
                Arg.Is<IEnumerable<InsertProblemAccountBySportEntity>>(x => x.Count() == 1),
                Arg.Any<CCInputFlow>(),
                BySport_FunctionId.InsertPACategory);

            Assert.False(result);
        }

        [Fact]
        public void InsertProblemAccount_WhenEmptyList_ShouldStillReturnFalse()
        {
            // Arrange
            var paBySports = new List<InsertProblemAccountBySportEntity>();
            var paras = new PushDataToExternalEntity
            {
                ServiceName = "TestService"
            };

            _problemAccountBySportService.InsertProblemAccountBySport(Arg.Any<IEnumerable<InsertProblemAccountBySportEntity>>(), Arg.Any<CCInputFlow>(), BySport_FunctionId.InsertPACategory)
                .Returns(Enumerable.Empty<InsertPABySportResultModel>());

            // Act
            var result = _service.InsertProblemAccount(paBySports, paras);

            // Assert
            _problemAccountBySportService.Received(1).InsertProblemAccountBySport(
                Arg.Any<IEnumerable<InsertProblemAccountBySportEntity>>(),
                Arg.Any<CCInputFlow>(),
                BySport_FunctionId.InsertPACategory);

            Assert.False(result);
        }

        [Fact]
        public void InsertProblemAccount_ThrowException()
        {
            // Arrange
            var paBySports = new List<InsertProblemAccountBySportEntity>
            {
                new InsertProblemAccountBySportEntity { CustID = 1 }
            };

            var paras = new PushDataToExternalEntity
            {
                ServiceName = "TestService"
            };

            _problemAccountBySportService.InsertProblemAccountBySport(Arg.Any<IEnumerable<InsertProblemAccountBySportEntity>>(), Arg.Any<CCInputFlow>(), BySport_FunctionId.InsertPACategory)
                .Throws<Exception>();

            // Act
            Action action = () => _service.InsertProblemAccount(paBySports, paras);

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion
    }
}
