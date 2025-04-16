using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer;
using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities;
using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    public class MissingCustomerServiceTest
    {
        private readonly IMissingCustomerDataAccess _dataAccess;

        private readonly IMissingCustomerService _service;

        public MissingCustomerServiceTest()
        {
            _dataAccess = Substitute.For<IMissingCustomerDataAccess>();

            _service = new MissingCustomerService(_dataAccess);
        }

        [Fact]
        public void GetLatencyCustomerFromQueue_WhenSuccess_ReturnData()
        {
            // Arrange
            var batchSize = 500;
            var expectedData = new List<int> { 1, 2, 3 };

            // Act
            _dataAccess.GetLatencyCustomerFromQueue(batchSize).Returns(expectedData);

            var result = _service.GetLatencyCustomerFromQueue(batchSize);

            // Assert
            _dataAccess.Received(1).GetLatencyCustomerFromQueue(batchSize);
            Assert.Equal(expectedData, result);
        }

        [Fact]
        public void GetLatencyCustomerFromQueue_ThrowException()
        {
            // Arrange
            var batchSize = 500;

            // Act
            _dataAccess.GetLatencyCustomerFromQueue(batchSize).Throws<Exception>();

            Action action = () => _service.GetLatencyCustomerFromQueue(batchSize);

            // Assert
            _dataAccess.Received(0).GetLatencyCustomerFromQueue(batchSize);
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void CompleteLatencyCustomerFromQueue_WhenSuccess_WithDataNotEmpty()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            _service.CompleteLatencyCustomerFromQueue(data);

            // Assert
            _dataAccess.Received(1).CompleteLatencyCustomerFromQueue(Arg.Any<string>());
        }

        [Fact]
        public void CompleteLatencyCustomerFromQueue_WhenSuccess_WithDataEmpty()
        {
            // Arrange
            var data = new List<int> { };

            // Act
            _service.CompleteLatencyCustomerFromQueue(data);

            // Assert
            _dataAccess.Received(0).CompleteLatencyCustomerFromQueue(Arg.Any<string>());
        }

        [Fact]
        public void CompleteLatencyCustomerFromQueue_ThrowException()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            _dataAccess.When(x => x.CompleteLatencyCustomerFromQueue(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.CompleteLatencyCustomerFromQueue(data);

            // Assert
            _dataAccess.Received(0).CompleteLatencyCustomerFromQueue(Arg.Any<string>());
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void InsertMissingCustomerLatencyToQueue_WhenSuccess_WithNotEmpty()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            _service.InsertMissingCustomerByLatencyToQueue(data);

            // Assert
            _dataAccess.Received(1).InsertMissingCustomerByLatencyToQueue(Arg.Any<string>());
        }

        [Fact]
        public void InsertMissingCustomerLatencyToQueue_WhenSuccess_WithEmpty()
        {
            // Arrange
            var data = new List<int> { };

            // Act
            _service.InsertMissingCustomerByLatencyToQueue(data);

            // Assert
            _dataAccess.Received(0).InsertMissingCustomerByLatencyToQueue(Arg.Any<string>());
        }

        [Fact]
        public void CheckMissingCustomerFromMainDB_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            var expectedData = new InsertCustomerModel
            {
                CustomerInfos = new List<CustomerMainDBEntity> { new CustomerMainDBEntity { CustID = 1 } },
                MissingCustIds = new List<int> { 2, 3 }
            };

            // Act
            _dataAccess.CheckCustomerInfoFromMainDB(Arg.Any<string>()).Returns(expectedData);

            var result = _service.CheckCustomerInfoFromMainDB(data);

            // Assert
            _dataAccess.Received(1).CheckCustomerInfoFromMainDB(Arg.Any<string>());
            Assert.Equal(expectedData, result);
        }

        [Fact]
        public void CheckMissingCustomerFromMainDB_WhenSuccess_ParamsNoData()
        {
            // Arrange
            var data = new List<int>();

            // Act
            var result = _service.CheckCustomerInfoFromMainDB(data);

            // Assert
            _dataAccess.Received(0).CheckCustomerInfoFromMainDB(Arg.Any<string>());
            Assert.NotNull(result);
        }

        [Fact]
        public void CheckMissingCustomerFromMainDB_ThrowException()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            _dataAccess.CheckCustomerInfoFromMainDB(Arg.Any<string>()).Throws<Exception>();

            Action action = () => _service.CheckCustomerInfoFromMainDB(data);

            // Assert
            _dataAccess.Received(0).CheckCustomerInfoFromMainDB(Arg.Any<string>());
            Assert.Throws<Exception>(action);
        }

        [Theory]
        [InlineData(120)]
        [InlineData(300)]
        public void GetLastScannedCustID_WhenSuccess_ReturnData(int scannedSecondTime)
        {
            // Arrange
            var data = 999;

            // Act
            _dataAccess.GetLastScannedCustIDByScannedSecondTime(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>()).Returns(data);

            var result = _service.GetLastScannedCustIDByScannedSecondTime(0, 999, scannedSecondTime);

            // Assert
            _dataAccess.Received(1).GetLastScannedCustIDByScannedSecondTime(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>());
            Assert.Equal(data, result);
        }

        [Theory]
        [InlineData(120)]
        [InlineData(300)]
        public void GetLastScannedCustID_ThrowException(int scannedSecondTime)
        {
            // Arrange

            // Act
            _dataAccess.GetLastScannedCustIDByScannedSecondTime(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>()).Throws<Exception>();

            Action action = () => _service.GetLastScannedCustIDByScannedSecondTime(0, 999, scannedSecondTime);

            // Assert
            _dataAccess.Received(0).GetLastScannedCustIDByScannedSecondTime(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>());
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void ScanLatencyCustomer_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new List<int>();

            // Act
            _dataAccess.ScanLatencyCustomer(123, 456, 500).Returns(new ScanLatencyCustomerEntity
            {
                CustIDs = data
            });

            var result = _service.ScanLatencyCustomer(123, 456, 500);

            // Assert
            _dataAccess.Received(1).ScanLatencyCustomer(123, 456, 500);
            Assert.Equal(data, result.CustIDs);
        }

        [Fact]
        public void CheckMissingCustomer_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            var expectedData = new List<int> { 1, 2 };

            // Act
            _dataAccess.CheckMissingCustomer(Arg.Any<string>()).Returns(expectedData);

            var result = _service.CheckMissingCustomer(data);

            // Assert
            _dataAccess.Received(1).CheckMissingCustomer(Arg.Any<string>());
            Assert.Equal(expectedData, result);
        }

        [Fact]
        public void CheckMissingCustomer_WhenSuccess_ReturnEmptyData()
        {
            // Arrange
            var data = new List<int>();

            // Act
            var result = _service.CheckMissingCustomer(data);

            // Assert
            _dataAccess.Received(0).CheckMissingCustomer(Arg.Any<string>());
            Assert.Equal(Enumerable.Empty<int>(), result);
        }
    }
}
