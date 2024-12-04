using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalDangerousAssociation
{
    public class NormalDangerousAssociationScanServiceTest
    {
        private readonly INormalDangerousAssociationScanDataAccess _dataAccess;

        private readonly INormalDangerousAssociationScanService _service;

        public NormalDangerousAssociationScanServiceTest()
        {
            _dataAccess = Substitute.For<INormalDangerousAssociationScanDataAccess>();
            _service = new NormalDangerousAssociationScanService(_dataAccess);
        }

        [Fact]
        public void CheckDangerousAssociationPerformance_WhenSuccess()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            // Act
            _service.CheckDangerousAssociationPerformance(custIds);

            // Assert
            _dataAccess.Received(1).CheckDangerousAssociationPerformance(Arg.Any<string>());
        }

        [Fact]
        public void CheckDangerousAssociationPerformance_WhenEmpty()
        {
            // Arrange
            var custIds = new List<long>();

            // Act
            _service.CheckDangerousAssociationPerformance(custIds);

            // Assert
            _dataAccess.Received(0).CheckDangerousAssociationPerformance(Arg.Any<string>());
        }

        [Fact]
        public void CheckDangerousAssociationPerformance_ThrowException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            // Act
            _dataAccess.When(x => x.CheckDangerousAssociationPerformance(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.CheckDangerousAssociationPerformance(custIds);

            // Assert
            _dataAccess.Received(0).CheckDangerousAssociationPerformance(Arg.Any<string>());

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void CompleteDangerousAssociation_WhenSuccess()
        {
            // Arrange

            // Act
            _service.CompleteDangerousAssociation(123);

            // Assert
            _dataAccess.Received(1).CompleteDangerousAssociation(Arg.Any<long>());
        }

        [Fact]
        public void CompleteDangerousAssociation_ThrowException()
        {
            // Arrange

            // Act
            _dataAccess.When(x => x.CompleteDangerousAssociation(Arg.Any<long>())).Throw<Exception>();

            Action aciton = () => _service.CompleteDangerousAssociation(123);

            // Assert
            _dataAccess.Received(0).CompleteDangerousAssociation(Arg.Any<long>());

            Assert.Throws<Exception>(aciton);
        }

        [Fact]
        public void InsertDangerousAssociationDataSource_WhenSuccess()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 4, 5 };

            // Act
            _service.InsertDangerousAssociationDataSource(custIds);

            // Assert
            _dataAccess.Received(1).InsertDangerousAssociationDataSource(Arg.Any<string>());
        }

        [Fact]
        public void InsertDangerousAssociationDataSource_WhenEmpty()
        {
            // Arrange
            var custIds = new List<long>();

            // Act
            _service.InsertDangerousAssociationDataSource(custIds);

            // Assert
            _dataAccess.Received(0).InsertDangerousAssociationDataSource(Arg.Any<string>());
        }

        [Fact]
        public void InsertDangerousAssociationDataSource_ThrowException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            // Act
            _dataAccess.When(x => x.InsertDangerousAssociationDataSource(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.InsertDangerousAssociationDataSource(custIds);

            // Assert
            _dataAccess.Received(0).InsertDangerousAssociationDataSource(Arg.Any<string>());

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void ScanDangerousAssociation_WhenSuccess()
        {
            // Arrange

            // Act
            _service.ScanDangerousAssociation(5000);

            // Assert
            _dataAccess.Received(1).ScanDangerousAssociation(Arg.Any<int>());
        }

        [Fact]
        public void ScanDangerousAssociation_ThrowException()
        {
            // Arrange

            // Act
            _dataAccess.ScanDangerousAssociation(Arg.Any<int>()).Throws<Exception>();

            Action action = () => _service.ScanDangerousAssociation(5000);

            // Assert
            _dataAccess.Received(0).ScanDangerousAssociation(Arg.Any<int>());

            Assert.Throws<Exception>(action);
        }
    }
}
