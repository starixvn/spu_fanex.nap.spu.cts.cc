using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.JobMonitoring
{
    public class CustomerJobMonitorTest
    {
        private readonly ICustomerJobMonitorDataAccess _customerJobMonitorDataAccess;
        private readonly IClassificationJobMonitorDataAccess _classificationJobMonitorDataAccess;

        private readonly ICustomerJobMonitorService _service;

        public CustomerJobMonitorTest()
        {
            _customerJobMonitorDataAccess = Substitute.For<ICustomerJobMonitorDataAccess>();
            _classificationJobMonitorDataAccess = Substitute.For<IClassificationJobMonitorDataAccess>();

            _service = new CustomerJobMonitorService(_customerJobMonitorDataAccess, _classificationJobMonitorDataAccess);
        }

        #region GetCustInfoFromMainDBForMonitor
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GetCustInfoFromMainDBForMonitor_WhenSuccess(long lastScannedCustID)
        {
            // Arrange

            // Act
            _customerJobMonitorDataAccess.GetCustInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>()).Returns(new MonitoringNewCustomerViewModel());

            var result = _service.GetCustInfoFromMainDBForMonitor(999, DateTime.MinValue, lastScannedCustID);

            // Assert
            _customerJobMonitorDataAccess.Received(lastScannedCustID != 0 ? 1 : 0).GetCustInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetCustInfoFromMainDBForMonitor_ThrowException()
        {
            // Arrange

            // Act
            _customerJobMonitorDataAccess.GetCustInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>()).Throws<Exception>();

            Action action = () => _service.GetCustInfoFromMainDBForMonitor(999, DateTime.MinValue, 1);

            // Assert
            _customerJobMonitorDataAccess.Received(0).GetCustInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region GetCustSubInfoFromMainDBForMonitor
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GetCustSubInfoFromMainDBForMonitor_WhenSuccess(long lastScannedCustID)
        {
            // Arrange

            // Act
            _customerJobMonitorDataAccess.GetCustSubInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>()).Returns(new MonitoringNewCustomerViewModel());

            var result = _service.GetCustSubInfoFromMainDBForMonitor(999, DateTime.MinValue, lastScannedCustID);

            // Assert
            _customerJobMonitorDataAccess.Received(lastScannedCustID != 0 ? 1 : 0).GetCustSubInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetCustSubInfoFromMainDBForMonitor_ThrowException()
        {
            // Arrange

            // Act
            _customerJobMonitorDataAccess.GetCustSubInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>()).Throws<Exception>();

            Action action = () => _service.GetCustSubInfoFromMainDBForMonitor(999, DateTime.MinValue, 1);

            // Assert
            _customerJobMonitorDataAccess.Received(0).GetCustSubInfoFromMainDBForMonitor(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<long>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region CheckMissingCustomer
        [Fact]
        public void CheckMissingCustomer_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new List<MonitoringNewCustomerEntity>
            {
                new MonitoringNewCustomerEntity { CustID = 1 }
            };

            // Act
            _customerJobMonitorDataAccess.CheckMissingCustomer(Arg.Any<string>()).Returns(new List<MissingCustomerCompareEntity>());

            var result = _service.CheckMissingCustomer(data);

            // Assert
            _customerJobMonitorDataAccess.Received(1).CheckMissingCustomer(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<List<MissingCustomerCompareEntity>>(result);
        }

        [Fact]
        public void CheckMissingCustomer_WhenSuccess_ReturnNull()
        {
            // Arrange
            var data = new List<MonitoringNewCustomerEntity>
            {
                new MonitoringNewCustomerEntity { CustID = 1 }
            };

            // Act
            _customerJobMonitorDataAccess.CheckMissingCustomer(Arg.Any<string>()).Returns((List<MissingCustomerCompareEntity>)null);

            var result = _service.CheckMissingCustomer(data);

            // Assert
            _customerJobMonitorDataAccess.Received(1).CheckMissingCustomer(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void CheckMissingCustomer_WhenSuccess_NoParamValue()
        {
            // Arrange

            // Act
            var result = _service.CheckMissingCustomer(Enumerable.Empty<MonitoringNewCustomerEntity>());

            // Assert
            _customerJobMonitorDataAccess.Received(0).CheckMissingCustomer(Arg.Any<string>());
            Assert.NotNull(result);
        }

        [Fact]
        public void CheckMissingCustomer_ThrowException()
        {
            // Arrange
            var data = new List<MonitoringNewCustomerEntity>
            {
                new MonitoringNewCustomerEntity { CustID = 1 }
            };

            // Act
            _customerJobMonitorDataAccess.CheckMissingCustomer(Arg.Any<string>()).Throws<Exception>();

            Action action = () => _service.CheckMissingCustomer(data);

            // Assert
            Assert.Throws<Exception>(action);
            _customerJobMonitorDataAccess.Received(1).CheckMissingCustomer(Arg.Any<string>());
        }
        #endregion

        #region CheckIncorrectCustomerInfo
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void CheckIncorrectCustomerInfo_WhenSuccess(ulong ctsCustID)
        {
            // Arrange

            // Act
            _customerJobMonitorDataAccess.CheckIncorrectCustomerInfo(Arg.Any<ulong>()).Returns(new IncorrectCustomerInfoModel());

            var result = _service.CheckIncorrectCustomerInfo(ctsCustID);

            // Assert
            _customerJobMonitorDataAccess.Received(ctsCustID != 0 ? 1 : 0).CheckIncorrectCustomerInfo(Arg.Any<ulong>());

            Assert.NotNull(result);
        }

        [Fact]
        public void CheckIncorrectCustomerInfo_ThrowException()
        {
            // Arrange

            // Act
            _customerJobMonitorDataAccess.CheckIncorrectCustomerInfo(Arg.Any<ulong>()).Throws<Exception>();

            Action action = () => _service.CheckIncorrectCustomerInfo(999);

            // Assert
            _customerJobMonitorDataAccess.Received(0).CheckIncorrectCustomerInfo(Arg.Any<ulong>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region GetCustomerCategoryToMonitor
        [Fact]
        public void GetCustomerCategoryToMonitor_WhenSuccess()
        {
            // Arrange
            var custIds = new List<long>() { 1, 2, 3, 4, 5 };
            var batchSize = 1;

            // Act
            _classificationJobMonitorDataAccess.GetCustomerCategoryToMonitor(Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<CustomerCategoryMonitorEntity> { new CustomerCategoryMonitorEntity() });

            var result = _service.GetCustomerCategoryToMonitor(custIds, batchSize);

            // Assert
            _classificationJobMonitorDataAccess.Received(5).GetCustomerCategoryToMonitor(Arg.Any<string>(), Arg.Any<bool>());

            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetCustomerCategoryToMonitor_WhenSuccess_NoParamValue()
        {
            // Arrange

            // Act
            var result = _service.GetCustomerCategoryToMonitor(Enumerable.Empty<long>(), 0);

            // Assert
            _classificationJobMonitorDataAccess.Received(0).GetCustomerCategoryToMonitor(Arg.Any<string>(), Arg.Any<bool>());

            Assert.NotNull(result);
        }

        [Fact]

        public void GetCustomerCategoryToMonitor_ThrowException()
        {
            // Arrange
            var custIds = new List<long>() { 1, 2, 3, 4, 5 };
            var batchSize = 1;

            // Act
            _classificationJobMonitorDataAccess.GetCustomerCategoryToMonitor(Arg.Any<string>(), Arg.Any<bool>()).Throws<Exception>();

            Action action = () => _service.GetCustomerCategoryToMonitor(custIds, batchSize);

            // Assert
            _classificationJobMonitorDataAccess.Received(0).GetCustomerCategoryToMonitor(Arg.Any<string>(), Arg.Any<bool>());

            Assert.Throws<Exception>(action);
        }
        #endregion
    }
}
