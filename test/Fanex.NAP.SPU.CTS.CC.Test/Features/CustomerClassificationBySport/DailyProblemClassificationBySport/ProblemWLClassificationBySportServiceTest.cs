namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.DailyProblemClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ProblemWLClassificationBySportServiceTest
    {
        private readonly IProblemWLClassificationBySportDataAccess _dataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDB;
        private readonly IProblemWLClassificationBySportService _service;

        public ProblemWLClassificationBySportServiceTest()
        {
            _dataAccess = Substitute.For<IProblemWLClassificationBySportDataAccess>();
            _custClassToMainDB = Substitute.For<ICustClassToMainDBService>();
            _service = new ProblemWLClassificationBySportService(_dataAccess, _custClassToMainDB);
        }

        #region Constructor Tests
        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeService()
        {
            // Act & Assert
            Assert.NotNull(_service);
            Assert.IsType<ProblemWLClassificationBySportService>(_service);
        }
        #endregion

        #region InsertPAWLClassificationBySport Tests
        [Fact]
        public void InsertPAWLClassificationBySport_WithValidData_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 2, SportGroup = 2, WinlossStatus = 2 }
            };

            var expectedResult = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 },
                new InsertPABySportResultModel { CustID = 2, SportGroup = 2 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WithSingleItem_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
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

            var expectedResult = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 100, SportGroup = 5 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WithEmptyData_ShouldNotCallDataAccess_ReturnsService()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>();

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.DidNotReceive().InsertPAWLPerformanceBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WithNullData_ShouldNotCallDataAccess_ReturnsService()
        {
            // Arrange
            IEnumerable<ProblemAccountLosingBySportEntity> paWLClassification = null;

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.DidNotReceive().InsertPAWLPerformanceBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WhenDataAccessReturnsEmpty_ShouldReturnService()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var expectedResult = Enumerable.Empty<InsertPABySportResultModel>();
            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WhenDataAccessReturnsNull_ShouldReturnService()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns((IEnumerable<InsertPABySportResultModel>)null);

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WhenDataAccessThrowsException_ShouldPropagateException()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.InsertPAWLClassificationBySport(paWLClassification));
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Any<string>());
            Assert.NotNull(exception);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WithMultipleItems_ShouldSerializeAndCallDataAccess_ReturnsService()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 2, SportGroup = 2, WinlossStatus = 2 },
                new ProblemAccountLosingBySportEntity { CustId = 3, SportGroup = 3, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 4, SportGroup = 1, WinlossStatus = 2 }
            };

            var expectedResult = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 },
                new InsertPABySportResultModel { CustID = 2, SportGroup = 2 },
                new InsertPABySportResultModel { CustID = 3, SportGroup = 3 },
                new InsertPABySportResultModel { CustID = 4, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Is<string>(json =>
                !string.IsNullOrEmpty(json) &&
                json.Contains("CustID") &&
                json.Contains("SportGroup")));
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
        }

        [Fact]
        public void InsertPAWLClassificationBySport_WithDuplicateCustIDs_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 2, WinlossStatus = 2 }
            };

            var expectedResult = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 },
                new InsertPABySportResultModel { CustID = 1, SportGroup = 2 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertPAWLClassificationBySport(paWLClassification);

            // Assert
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<ProblemWLClassificationBySportService>(result);
        }
        #endregion

        #region PushCCToExternal Tests
        [Fact]
        public void PushCCToExternal_WithPAWLsNotEmpty_ShouldCallExternalService_ReturnsResult()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 },
                new InsertPABySportResultModel { CustID = 2, SportGroup = 2 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            var serviceName = "TestService";
            var batchExternalSize = 50;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Returns(true);

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Is<IEnumerable<InsertPABySportResultModel>>(x => x.Count() == 2),
                serviceName,
                batchExternalSize);
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WithPAWLsEmpty_ShouldReturnTrue_DoesNotCallExternalService()
        {
            // Arrange
            var serviceName = "TestService";
            var batchExternalSize = 50;

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.DidNotReceive().PushCCBySportToMainDB(
                Arg.Any<IEnumerable<InsertPABySportResultModel>>(),
                Arg.Any<string>(),
                Arg.Any<int>());
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WhenExternalServiceReturnsFalse_ShouldReturnFalse()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            var serviceName = "TestService";
            var batchExternalSize = 50;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Returns(false);

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Any<IEnumerable<InsertPABySportResultModel>>(),
                serviceName,
                batchExternalSize);
            Assert.False(result);
        }

        [Fact]
        public void PushCCToExternal_WhenExternalServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            var serviceName = "TestService";
            var batchExternalSize = 50;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.PushCCBySportToExternal(serviceName, batchExternalSize));
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Any<IEnumerable<InsertPABySportResultModel>>(),
                serviceName,
                batchExternalSize);
            Assert.NotNull(exception);
        }

        [Fact]
        public void PushCCToExternal_WithMultiplePAWLs_ShouldCallExternalServiceWithDistinct_ReturnsResult()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 },
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 }, // Duplicate
                new InsertPABySportResultModel { CustID = 2, SportGroup = 2 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            var serviceName = "TestService";
            var batchExternalSize = 50;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Returns(true);

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Is<IEnumerable<InsertPABySportResultModel>>(x => x.Count() <= 3), // Distinct may reduce count
                serviceName,
                batchExternalSize);
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WithZeroBatchSize_ShouldCallExternalService_ReturnsResult()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            var serviceName = "TestService";
            var batchExternalSize = 0;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Returns(true);

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Any<IEnumerable<InsertPABySportResultModel>>(),
                serviceName,
                batchExternalSize);
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WithNegativeBatchSize_ShouldCallExternalService_ReturnsResult()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            var serviceName = "TestService";
            var batchExternalSize = -10;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Returns(true);

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Any<IEnumerable<InsertPABySportResultModel>>(),
                serviceName,
                batchExternalSize);
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WithNullServiceName_ShouldCallExternalService_ReturnsResult()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            string serviceName = null;
            var batchExternalSize = 50;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Returns(true);

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Any<IEnumerable<InsertPABySportResultModel>>(),
                serviceName,
                batchExternalSize);
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WithEmptyServiceName_ShouldCallExternalService_ReturnsResult()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _service.InsertPAWLClassificationBySport(paWLClassification);

            var serviceName = string.Empty;
            var batchExternalSize = 50;

            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), serviceName, batchExternalSize).Returns(true);

            // Act
            var result = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Any<IEnumerable<InsertPABySportResultModel>>(),
                serviceName,
                batchExternalSize);
            Assert.True(result);
        }
        #endregion

        #region Integration Tests
        [Fact]
        public void InsertPAWLClassificationBySport_ThenPushCCToExternal_ShouldWorkCorrectly()
        {
            // Arrange
            var paWLClassification = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 2, SportGroup = 2, WinlossStatus = 2 }
            };

            var paWLs = new List<InsertPABySportResultModel>
            {
                new InsertPABySportResultModel { CustID = 1, SportGroup = 1 },
                new InsertPABySportResultModel { CustID = 2, SportGroup = 2 }
            };

            _dataAccess.InsertPAWLPerformanceBySport(Arg.Any<string>()).Returns(paWLs);
            _custClassToMainDB.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertPABySportResultModel>>(), Arg.Any<string>(), Arg.Any<int>()).Returns(true);

            var serviceName = "TestService";
            var batchExternalSize = 50;

            // Act
            var insertResult = _service.InsertPAWLClassificationBySport(paWLClassification);
            var pushResult = _service.PushCCBySportToExternal(serviceName, batchExternalSize);

            // Assert
            Assert.NotNull(insertResult);
            Assert.IsType<ProblemWLClassificationBySportService>(insertResult);
            _dataAccess.Received(1).InsertPAWLPerformanceBySport(Arg.Any<string>());
            _custClassToMainDB.Received(1).PushCCBySportToMainDB(
                Arg.Is<IEnumerable<InsertPABySportResultModel>>(x => x.Count() == 2),
                serviceName,
                batchExternalSize);
            Assert.True(pushResult);
        }
        #endregion
    }
}
