namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ProblemAccountBySportServiceTest
    {
        private readonly IProblemAccountBySportDataAccess _dataAccess;
        private readonly IProblemAccountBySportService _service;

        public ProblemAccountBySportServiceTest()
        {
            _dataAccess = Substitute.For<IProblemAccountBySportDataAccess>();
            _service = new ProblemAccountBySportService(_dataAccess);
        }

        #region Constructor Tests
        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeService()
        {
            // Act & Assert
            Assert.NotNull(_service);
            Assert.IsType<ProblemAccountBySportService>(_service);
        }
        #endregion

        #region GetPALosingBySport Tests
        [Fact]
        public void GetPALosingBySport_WithValidCustInfos_ShouldCallDataAccess_ReturnsData()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 1 },
                new InsertProblemAccountInfoBySportModel { CustID = 2, SportGroup = 2 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1, TurnoverRM = 1000, WinlossRM = -500 },
                new ProblemAccountLosingBySportEntity { CustId = 2, SportGroup = 2, WinlossStatus = 2, TurnoverRM = 2000, WinlossRM = -1000 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().CustId);
            Assert.Equal(2, result.Last().CustId);
        }

        [Fact]
        public void GetPALosingBySport_WithSingleCustInfo_ShouldCallDataAccess_ReturnsData()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 100, SportGroup = 5 }
            };

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

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(100, result.First().CustId);
            Assert.Equal(5, result.First().SportGroup);
        }

        [Fact]
        public void GetPALosingBySport_WithEmptyCustInfos_ShouldReturnEmpty_DoesNotCallDataAccess()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>();

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.DidNotReceive().GetPALosingBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetPALosingBySport_WithNullCustInfos_ShouldReturnEmpty_DoesNotCallDataAccess()
        {
            // Arrange
            IEnumerable<InsertProblemAccountInfoBySportModel> custInfos = null;

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.DidNotReceive().GetPALosingBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetPALosingBySport_WhenDataAccessReturnsEmpty_ShouldReturnEmpty()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 1 }
            };

            var expectedResult = Enumerable.Empty<ProblemAccountLosingBySportEntity>();
            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetPALosingBySport_WhenDataAccessReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns((IEnumerable<ProblemAccountLosingBySportEntity>)null);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.Null(result);
        }

        [Fact]
        public void GetPALosingBySport_WhenDataAccessThrowsException_ShouldPropagateException()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.GetPALosingBySport(custInfos));
            _dataAccess.Received(1).GetPALosingBySport(Arg.Any<string>());
            Assert.NotNull(exception);
        }

        [Fact]
        public void GetPALosingBySport_WhenDataAccessThrowsDatabaseException_ShouldPropagateException()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 1 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Throws<Exception>();
            // Act
            var action = () => _service.GetPALosingBySport(custInfos);

            // Act & Assert
            Assert.Throws<Exception>(action);
            _dataAccess.Received(1).GetPALosingBySport(Arg.Any<string>());
        }

        [Fact]
        public void GetPALosingBySport_WithMultipleCustInfos_ShouldSerializeAndCallDataAccess_ReturnsAllData()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 1 },
                new InsertProblemAccountInfoBySportModel { CustID = 2, SportGroup = 2 },
                new InsertProblemAccountInfoBySportModel { CustID = 3, SportGroup = 3 },
                new InsertProblemAccountInfoBySportModel { CustID = 4, SportGroup = 1 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 2, SportGroup = 2, WinlossStatus = 2 },
                new ProblemAccountLosingBySportEntity { CustId = 3, SportGroup = 3, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 4, SportGroup = 1, WinlossStatus = 2 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Is<string>(json =>
                !string.IsNullOrEmpty(json) &&
                json.Contains("CustID") &&
                json.Contains("SportGroup")));
            Assert.NotNull(result);
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void GetPALosingBySport_WithCustInfosHavingSameCustID_ShouldCallDataAccess_ReturnsData()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 1 },
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 2 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 1, WinlossStatus = 1 },
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 2, WinlossStatus = 2 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal(1, r.CustId));
        }

        [Fact]
        public void GetPALosingBySport_WithZeroCustID_ShouldCallDataAccess_ReturnsData()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 0, SportGroup = 1 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 0, SportGroup = 1, WinlossStatus = 1 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(0, result.First().CustId);
        }

        [Fact]
        public void GetPALosingBySport_WithZeroSportGroup_ShouldCallDataAccess_ReturnsData()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = 1, SportGroup = 0 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = 1, SportGroup = 0, WinlossStatus = 1 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(0, result.First().SportGroup);
        }

        [Fact]
        public void GetPALosingBySport_WithNegativeCustID_ShouldCallDataAccess_ReturnsData()
        {
            // Arrange
            var custInfos = new List<InsertProblemAccountInfoBySportModel>
            {
                new InsertProblemAccountInfoBySportModel { CustID = -1, SportGroup = 1 }
            };

            var expectedResult = new List<ProblemAccountLosingBySportEntity>
            {
                new ProblemAccountLosingBySportEntity { CustId = -1, SportGroup = 1, WinlossStatus = 1 }
            };

            _dataAccess.GetPALosingBySport(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.GetPALosingBySport(custInfos);

            // Assert
            _dataAccess.Received(1).GetPALosingBySport(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(-1, result.First().CustId);
        }
        #endregion
    }
}
