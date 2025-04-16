using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities.ScanLatencyCustomer;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    public class ScanLatencyCustomerTest
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;

        public ScanLatencyCustomerTest()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, custClassToMainDBService);
        }

        [Theory]
        [InlineData(120)]
        [InlineData(300)]
        public void GetLastScannedCustID_WhenSuccess_ReturnData(int scannedSecondTime)
        {
            // Arrange
            var data = 999;

            // Act
            dataAccess.GetLastScannedCustID(Arg.Any<int>()).Returns(data);

            var result = ctsCustomerService.GetLastScannedCustID(scannedSecondTime);

            // Assert
            dataAccess.Received(1).GetLastScannedCustID(Arg.Any<int>());
            Assert.Equal(data, result);
        }

        [Theory]
        [InlineData(120)]
        [InlineData(300)]
        public void GetLastScannedCustID_ThrowException(int scannedSecondTime)
        {
            // Arrange

            // Act
            dataAccess.GetLastScannedCustID(Arg.Any<int>()).Throws<Exception>();

            Action action = () => ctsCustomerService.GetLastScannedCustID(scannedSecondTime);

            // Assert
            dataAccess.Received(0).GetLastScannedCustID(Arg.Any<int>());
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void ScanLatencyCustomer_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new List<int>();

            // Act
            dataAccess.ScanLatencyCustomer(123, 456).Returns(new ScanLatencyCustomerEntity
            {
                CustIDs = data
            });

            var result = ctsCustomerService.ScanLatencyCustomer(123, 456);

            // Assert
            dataAccess.Received(1).ScanLatencyCustomer(123, 456);
            Assert.Equal(data, result.CustIDs);
        }

        [Fact]
        public void CheckMissingCustomer_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            var expectedData = new List<int> { 1, 2 };

            // Act
            dataAccess.CheckMissingCustomer(Arg.Any<string>()).Returns(expectedData);

            var result = ctsCustomerService.CheckMissingCustomer(data);

            // Assert
            dataAccess.Received(1).CheckMissingCustomer(Arg.Any<string>());
            Assert.Equal(expectedData, result);
        }

        [Fact]
        public void CheckMissingCustomer_WhenSuccess_ReturnEmptyData()
        {
            // Arrange
            var data = new List<int>();

            // Act
            var result = ctsCustomerService.CheckMissingCustomer(data);

            // Assert
            dataAccess.Received(0).CheckMissingCustomer(Arg.Any<string>());
            Assert.Equal(Enumerable.Empty<int>(), result);
        }
    }
}
