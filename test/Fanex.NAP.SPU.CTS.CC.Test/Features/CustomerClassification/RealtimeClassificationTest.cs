namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.RealTimeClassificationConsts;

    public class RealtimeClassificationTest
    {
        private readonly IRealtimeClassificationDataAccess _dataAccess;
        private readonly ISentryClient _sentryClient;

        private readonly IRealtimeClassificationService _service;

        public RealtimeClassificationTest()
        {
            _dataAccess = Substitute.For<IRealtimeClassificationDataAccess>();
            _sentryClient = Substitute.For<ISentryClient>();

            _service = new RealtimeClassificationService(_dataAccess, _sentryClient);
        }

        #region GetNewPlaceBetCustomers
        [Fact]
        public void GetNewPlaceBetCustomers_WhenSuccess()
        {
            // Arrange
            var customersField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.Customers);
            customersField.SetValue(_service, new GetNewPlaceBetCustomerModel());

            // Act
            _dataAccess.GetNewPlaceBetCustomers().Returns(new GetNewPlaceBetCustomerModel());

            _service.GetNewPlaceBetCustomers();
            // Assert
            _dataAccess.Received(1).GetNewPlaceBetCustomers();
        }

        [Fact]
        public void GetNewPlaceBetCustomers_ThrowException()
        {
            // Arrange
            var customersField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.Customers);
            customersField.SetValue(_service, new GetNewPlaceBetCustomerModel());

            // Act
            _dataAccess.GetNewPlaceBetCustomers().Throws<Exception>();

            Action action = () => _service.GetNewPlaceBetCustomers();
            // Assert
            _dataAccess.Received(0).GetNewPlaceBetCustomers();

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region GetCustomerCategory
        [Fact]
        public void GetCustomerCategory_WhenSuccess()
        {
            // Arrange
            var customersField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.Customers);
            customersField.SetValue(_service,
                new GetNewPlaceBetCustomerModel()
                {
                    CustomerNewPlaceBets = new List<GetNewPlaceBetCustomerEntity>()
                    {
                        new GetNewPlaceBetCustomerEntity() { CustId = 1 },
                        new GetNewPlaceBetCustomerEntity() { CustId = 2 },
                        new GetNewPlaceBetCustomerEntity() { CustId = 3 },
                        new GetNewPlaceBetCustomerEntity() { CustId = 4 },
                    }
                });

            var customerCategoriesField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.CustomerCategories);
            customerCategoriesField.SetValue(_service, new GetCustomerCategoryRealtimeModel());

            var batchSize = 1;

            // Act
            _dataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(new GetCustomerCategoryRealtimeModel());

            _service.GetCustomerCategory(batchSize);
            // Assert
            _dataAccess.Received(4).GetCustomerCategory(Arg.Any<string>());
        }

        [Fact]
        public void GetCustomerCategory_ThrowException()
        {
            // Arrange
            var customersField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.Customers);
            customersField.SetValue(_service,
                new GetNewPlaceBetCustomerModel()
                {
                    CustomerNewPlaceBets = new List<GetNewPlaceBetCustomerEntity>()
                    {
                        new GetNewPlaceBetCustomerEntity() { CustId = 1 },
                        new GetNewPlaceBetCustomerEntity() { CustId = 2 },
                        new GetNewPlaceBetCustomerEntity() { CustId = 3 },
                        new GetNewPlaceBetCustomerEntity() { CustId = 4 },
                    }
                });

            var customerCategoriesField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.CustomerCategories);
            customerCategoriesField.SetValue(_service, new GetCustomerCategoryRealtimeModel());

            var batchSize = 1;

            // Act
            _dataAccess.GetCustomerCategory(Arg.Any<string>()).Throws<Exception>();

            Action action = () => _service.GetCustomerCategory(batchSize);

            // Assert
            _dataAccess.Received(0).GetCustomerCategory(Arg.Any<string>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region PreprocessCustomerClassRealtime
        [Fact]
        public void PreprocessCustomerClassRealtime_WhenSuccess()
        {
            // Arrange
            var customerCategoriesField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.CustomerCategories);
            customerCategoriesField.SetValue(_service,
                    new GetCustomerCategoryRealtimeModel()
                    {
                        CustomerCategory = new List<long> { 1, 2, 3, 4 }
                    }
                );

            var batchSize = 1;

            // Act
            _dataAccess.PreprocessCustomerClassRealtime(Arg.Any<string>());

            _service.PreprocessCustomerClassRealtime(batchSize);
            // Assert
            _dataAccess.Received(4).PreprocessCustomerClassRealtime(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCustomerClassRealtime_ThrowException()
        {
            // Arrange
            var customerCategoriesField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.CustomerCategories);
            customerCategoriesField.SetValue(_service,
                    new GetCustomerCategoryRealtimeModel()
                    {
                        CustomerCategory = new List<long> { 1, 2, 3, 4 }
                    }
                );

            var batchSize = 1;

            // Act
            _dataAccess.When(x => x.PreprocessCustomerClassRealtime(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.PreprocessCustomerClassRealtime(batchSize);

            // Assert
            _dataAccess.Received(0).PreprocessCustomerClassRealtime(Arg.Any<string>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertPADailyScanQueue
        [Fact]
        public void InsertPADailyScanQueue_WhenSuccess()
        {
            // Arrange
            var customerCategoriesField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.CustomerCategories);
            customerCategoriesField.SetValue(_service,
                    new GetCustomerCategoryRealtimeModel()
                    {
                        CustomerPADailyScan = new List<long> { 1, 2, 3, 4 }
                    }
                );

            var batchSize = 1;

            // Act
            _dataAccess.InsertPADailyScanQueue(Arg.Any<string>());

            _service.InsertPADailyScanQueue(batchSize);
            // Assert
            _dataAccess.Received(4).InsertPADailyScanQueue(Arg.Any<string>());
        }

        [Fact]
        public void InsertPADailyScanQueue_ThrowException()
        {
            // Arrange
            var customerCategoriesField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.CustomerCategories);
            customerCategoriesField.SetValue(_service,
                    new GetCustomerCategoryRealtimeModel()
                    {
                        CustomerPADailyScan = new List<long> { 1, 2, 3, 4 }
                    }
                );

            var batchSize = 1;

            // Act
            _dataAccess.When(x => x.InsertPADailyScanQueue(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.InsertPADailyScanQueue(batchSize);

            // Assert
            _dataAccess.Received(0).InsertPADailyScanQueue(Arg.Any<string>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region CompleteCustomerRealtime
        [Fact]
        public void CompleteCustomerRealtime_WhenSuccess()
        {
            // Arrange
            var customersField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.Customers);
            customersField.SetValue(_service,
                new GetNewPlaceBetCustomerModel()
                {
                    ScannedMaxId = 999
                });

            // Act
            _dataAccess.CompleteCustomerRealtime(Arg.Any<long>());

            _service.CompleteCustomerRealtime();
            // Assert
            _dataAccess.Received(1).CompleteCustomerRealtime(Arg.Any<long>());
        }

        [Fact]
        public void CompleteCustomerRealtime_ThrowException()
        {
            // Arrange
            var customersField = TestExtension.GetPrivateFieldInfo(typeof(RealtimeClassificationService), PrivateFieldName.Customers);
            customersField.SetValue(_service,
                new GetNewPlaceBetCustomerModel()
                {
                    ScannedMaxId = 999
                });

            // Act
            _dataAccess.When(x => x.CompleteCustomerRealtime(Arg.Any<long>())).Throw<Exception>();

            Action action = () => _service.CompleteCustomerRealtime();

            // Assert
            _dataAccess.Received(0).CompleteCustomerRealtime(Arg.Any<long>());
            Assert.Throws<Exception>(action);
        }
        #endregion
    }
}