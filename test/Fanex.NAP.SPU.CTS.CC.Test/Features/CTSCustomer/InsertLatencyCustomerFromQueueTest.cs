using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sentry;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    public class InsertLatencyCustomerFromQueueTest
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;

        public InsertLatencyCustomerFromQueueTest()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, custClassToMainDBService);
        }

        [Fact]
        public void GetLatencyCustomerFromQueue_WhenSuccess_ReturnData()
        {
            // Arrange
            var batchSize = 500;
            var expectedData = new List<int> { 1, 2, 3 };

            // Act
            dataAccess.GetLatencyCustomerFromQueue(batchSize).Returns(expectedData);

            var result = ctsCustomerService.GetLatencyCustomerFromQueue(batchSize);

            // Assert
            dataAccess.Received(1).GetLatencyCustomerFromQueue(batchSize);
            Assert.Equal(expectedData, result);
        }

        [Fact]
        public void GetLatencyCustomerFromQueue_ThrowException()
        {
            // Arrange
            var batchSize = 500;

            // Act
            dataAccess.GetLatencyCustomerFromQueue(batchSize).Throws<Exception>();

            Action action = () => ctsCustomerService.GetLatencyCustomerFromQueue(batchSize);

            // Assert
            dataAccess.Received(0).GetLatencyCustomerFromQueue(batchSize);
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void CompleteLatencyCustomerFromQueue_WhenSuccess_WithDataNotEmpty()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            ctsCustomerService.CompleteLatencyCustomerFromQueue(data);

            // Assert
            dataAccess.Received(1).CompleteLatencyCustomerFromQueue(Arg.Any<string>());
        }

        [Fact]
        public void CompleteLatencyCustomerFromQueue_WhenSuccess_WithDataEmpty()
        {
            // Arrange
            var data = new List<int> { };

            // Act
            ctsCustomerService.CompleteLatencyCustomerFromQueue(data);

            // Assert
            dataAccess.Received(0).CompleteLatencyCustomerFromQueue(Arg.Any<string>());
        }

        [Fact]
        public void CompleteLatencyCustomerFromQueue_ThrowException()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            dataAccess.When(x => x.CompleteLatencyCustomerFromQueue(Arg.Any<string>())).Throw<Exception>();

            Action action = () => ctsCustomerService.CompleteLatencyCustomerFromQueue(data);

            // Assert
            dataAccess.Received(0).CompleteLatencyCustomerFromQueue(Arg.Any<string>());
            Assert.Throws<Exception>(action);
        }
    }
}
