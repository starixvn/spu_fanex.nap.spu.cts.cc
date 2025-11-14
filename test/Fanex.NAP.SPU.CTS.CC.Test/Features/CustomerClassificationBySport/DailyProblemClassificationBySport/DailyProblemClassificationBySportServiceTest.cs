namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.DailyProblemClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DailyProblemClassificationBySportServiceTest
    {
        private readonly IProblemWLClassificationBySportDataAccess _dataAccess;
        private readonly IProblemAccountBySportService _paService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDailyProblemClassificationBySportService _service;

        public DailyProblemClassificationBySportServiceTest()
        {
            _dataAccess = Substitute.For<IProblemWLClassificationBySportDataAccess>();
            _paService = Substitute.For<IProblemAccountBySportService>();
            _serviceProvider = Substitute.For<IServiceProvider>();
            _service = new DailyProblemClassificationBySportService(_dataAccess, _serviceProvider, _paService);
        }

        #region Constructor Tests
        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeService()
        {
            // Act & Assert
            Assert.NotNull(_service);
            Assert.IsType<DailyProblemClassificationBySportService>(_service);
        }
        #endregion

        #region Start Method Tests
        [Fact]
        public void Start_ShouldInitializePACustomers_ReturnsService()
        {
            // Act
            var result = _service.Start();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationBySportService>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void Start_ShouldResetPACustomers_ReturnsService()
        {
            // Arrange - First call Start to set initial state
            _service.Start();

            // Act
            var result = _service.Start();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationBySportService>(result);
            Assert.Same(_service, result);
        }
        #endregion

        #region GetDailyPAFromQueue Tests
        [Fact]
        public void GetDailyPAFromQueue_WhenSuccess_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var expectedData = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 2, SportGroup = 2 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(expectedData);

            // Act
            var result = _service.GetDailyPAFromQueue(500);

            // Assert
            _dataAccess.Received(1).GetPADailyFromQueue(500);
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationBySportService>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void GetDailyPAFromQueue_WhenDataAccessReturnsEmpty_ShouldReturnService()
        {
            // Arrange
            var expectedData = new List<ProblemAccountInfoFromDailyQueueModel>();
            _dataAccess.GetPADailyFromQueue(500).Returns(expectedData);

            // Act
            var result = _service.GetDailyPAFromQueue(500);

            // Assert
            _dataAccess.Received(1).GetPADailyFromQueue(500);
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationBySportService>(result);
        }

        [Fact]
        public void GetDailyPAFromQueue_WhenDataAccessReturnsNull_ShouldReturnService()
        {
            // Arrange
            _dataAccess.GetPADailyFromQueue(500).Returns((IEnumerable<ProblemAccountInfoFromDailyQueueModel>)null);

            // Act
            var result = _service.GetDailyPAFromQueue(500);

            // Assert
            _dataAccess.Received(1).GetPADailyFromQueue(500);
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationBySportService>(result);
        }

        [Fact]
        public void GetDailyPAFromQueue_WhenDataAccessThrowsException_ShouldPropagateException()
        {
            // Arrange
            _dataAccess.GetPADailyFromQueue(500).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.GetDailyPAFromQueue(500));
            _dataAccess.Received(1).GetPADailyFromQueue(500);
            Assert.NotNull(exception);
        }

        [Fact]
        public void GetDailyPAFromQueue_WithMultipleItems_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var expectedData = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 2, SportGroup = 2 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 3, SportGroup = 3 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 4, SportGroup = 1 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(expectedData);

            // Act
            var result = _service.GetDailyPAFromQueue(500);

            // Assert
            _dataAccess.Received(1).GetPADailyFromQueue(500);
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationBySportService>(result);
        }
        #endregion

        #region GetProblemAccountLosingClassification Tests
        [Fact]
        public void GetProblemAccountLosingClassification_WithValidPACustomers_ShouldCallPAService_ReturnsData()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 2, SportGroup = 2 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 2, SportGroup = 2, WinlossStatus = 2 }
            };

            _paService.GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>()).Returns(expectedResult);

            // Act
            var result = _service.GetProblemAccountLosingClassification();

            // Assert
            _paService.Received(1).GetPALosingBySport(Arg.Is<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>(x => x.Count() == 2));
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().CustId);
            Assert.Equal(2, result.Last().CustId);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WithEmptyPACustomers_ShouldReturnEmpty_DoesNotCallPAService()
        {
            // Arrange
            _service.Start();

            // Act
            var result = _service.GetProblemAccountLosingClassification();

            // Assert
            _paService.DidNotReceive().GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WithNullPACustomers_ShouldReturnEmpty_DoesNotCallPAService()
        {
            // Arrange
            _dataAccess.GetPADailyFromQueue(500).Returns((IEnumerable<ProblemAccountInfoFromDailyQueueModel>)null);
            _service.GetDailyPAFromQueue(500);

            // Act
            var result = _service.GetProblemAccountLosingClassification();

            // Assert
            _paService.DidNotReceive().GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WhenPAServiceReturnsEmpty_ShouldReturnEmpty()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            var expectedResult = Enumerable.Empty<ProblemAccountLosingBySportEntity>();
            _paService.GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>()).Returns(expectedResult);

            // Act
            var result = _service.GetProblemAccountLosingClassification();

            // Assert
            _paService.Received(1).GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WhenPAServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            _paService.GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>()).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.GetProblemAccountLosingClassification());
            _paService.Received(1).GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>());
            Assert.NotNull(exception);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WithSinglePACustomer_ShouldCallPAService_ReturnsData()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 100, SportGroup = 5 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity
                {
                    CustId = 100,
                    SportGroup = 5,
                    WinlossStatus = 1,
                    TurnoverRM = 5000,
                    WinlossRM = -2500,
                    BetCount = 50,
                    ActiveDays = 10,
                    PerformanceTime = DateTime.Now
                }
            };

            _paService.GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>()).Returns(expectedResult);

            // Act
            var result = _service.GetProblemAccountLosingClassification();

            // Assert
            _paService.Received(1).GetPALosingBySport(Arg.Is<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>(x => x.Count() == 1));
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(100, result.First().CustId);
        }
        #endregion

        #region GetInstanceProblemWLClassificationBySportService Tests
        [Fact]
        public void GetInstanceProblemWLClassificationBySportService_WhenServiceProviderReturnsService_ShouldReturnService()
        {
            // Arrange
            var expectedService = Substitute.For<IProblemWLClassificationBySportService>();
            _serviceProvider.GetService(typeof(IProblemWLClassificationBySportService)).Returns(expectedService);

            // Act
            var result = _service.GetInstanceProblemWLClassificationBySportService();

            // Assert
            Assert.NotNull(result);
            Assert.Same(expectedService, result);
        }

        [Fact]
        public void GetInstanceProblemWLClassificationBySportService_WhenServiceProviderReturnsNull_ShouldReturnNull()
        {
            // Arrange
            _serviceProvider.GetService(typeof(IProblemWLClassificationBySportService)).Returns(null);

            // Act
            var result = _service.GetInstanceProblemWLClassificationBySportService();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetInstanceProblemWLClassificationBySportService_WhenServiceProviderThrowsException_ShouldPropagateException()
        {
            // Arrange
            _serviceProvider.GetService(typeof(IProblemWLClassificationBySportService)).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.GetInstanceProblemWLClassificationBySportService());
            Assert.NotNull(exception);
        }
        #endregion

        #region UpdateComplete Tests
        [Fact]
        public void UpdateComplete_WithValidPACustomers_ShouldCallDataAccess_ReturnsTrue()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 2, SportGroup = 2 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dataAccess.Received(1).CompletePADailyFromQueue(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.True(result);
        }

        [Fact]
        public void UpdateComplete_WithEmptyPACustomers_ShouldNotCallDataAccess_ReturnsFalse()
        {
            // Arrange
            _service.Start();

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dataAccess.DidNotReceive().CompletePADailyFromQueue(Arg.Any<string>());
            Assert.False(result);
        }

        [Fact]
        public void UpdateComplete_WithNullPACustomers_ShouldNotCallDataAccess_ReturnsFalse()
        {
            // Arrange
            _dataAccess.GetPADailyFromQueue(500).Returns((IEnumerable<ProblemAccountInfoFromDailyQueueModel>)null);
            _service.GetDailyPAFromQueue(500);

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dataAccess.DidNotReceive().CompletePADailyFromQueue(Arg.Any<string>());
            Assert.False(result);
        }

        [Fact]
        public void UpdateComplete_WhenDataAccessThrowsException_ShouldPropagateException()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            _dataAccess.When(x => x.CompletePADailyFromQueue(Arg.Any<string>())).Throw<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.UpdateComplete());
            _dataAccess.Received(1).CompletePADailyFromQueue(Arg.Any<string>());
            Assert.NotNull(exception);
        }

        [Fact]
        public void UpdateComplete_WithSinglePACustomer_ShouldSerializeAndCallDataAccess_ReturnsTrue()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 100, SportGroup = 5 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dataAccess.Received(1).CompletePADailyFromQueue(Arg.Is<string>(json =>
                !string.IsNullOrEmpty(json) &&
                json.Contains("CustID") &&
                json.Contains("SportGroup")));
            Assert.True(result);
        }

        [Fact]
        public void UpdateComplete_WithMultiplePACustomers_ShouldSerializeAndCallDataAccess_ReturnsTrue()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 2, SportGroup = 2 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 3, SportGroup = 3 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 } // Duplicate
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _service.GetDailyPAFromQueue(500);

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dataAccess.Received(1).CompletePADailyFromQueue(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.True(result);
        }
        #endregion

        #region Integration Tests
        [Fact]
        public void Start_ThenGetDailyPAFromQueue_ThenGetProblemAccountLosingClassification_ShouldWorkCorrectly()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 },
                new ProblemAccountInfoFromDailyQueueModel { CustID = 2, SportGroup = 2 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 2, SportGroup = 2, WinlossStatus = 2 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _paService.GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>()).Returns(expectedResult);

            // Act
            var startResult = _service.Start();
            var queueResult = _service.GetDailyPAFromQueue(500);
            var classificationResult = _service.GetProblemAccountLosingClassification();

            // Assert
            Assert.NotNull(startResult);
            Assert.NotNull(queueResult);
            Assert.NotNull(classificationResult);
            Assert.Equal(2, classificationResult.Count());
            _dataAccess.Received(1).GetPADailyFromQueue(500);
            _paService.Received(1).GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>());
        }

        [Fact]
        public void FullWorkflow_Start_GetDailyPAFromQueue_GetProblemAccountLosingClassification_UpdateComplete_ShouldWorkCorrectly()
        {
            // Arrange
            var paCustomers = new List<ProblemAccountInfoFromDailyQueueModel>
            {
                new ProblemAccountInfoFromDailyQueueModel { CustID = 1, SportGroup = 1 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 }
            };

            _dataAccess.GetPADailyFromQueue(500).Returns(paCustomers);
            _paService.GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>()).Returns(expectedResult);

            // Act
            _service.Start();
            _service.GetDailyPAFromQueue(500);
            var classificationResult = _service.GetProblemAccountLosingClassification();
            var updateResult = _service.UpdateComplete();

            // Assert
            Assert.NotNull(classificationResult);
            Assert.Single(classificationResult);
            Assert.True(updateResult);
            _dataAccess.Received(1).GetPADailyFromQueue(500);
            _dataAccess.Received(1).CompletePADailyFromQueue(Arg.Any<string>());
            _paService.Received(1).GetPALosingBySport(Arg.Any<IEnumerable<ProblemAccountInfoFromDailyQueueModel>>());
        }
        #endregion
    }
}
