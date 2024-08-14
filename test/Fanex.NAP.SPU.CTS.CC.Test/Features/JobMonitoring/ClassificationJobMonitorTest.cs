using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
using Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.ClassificationJobMonitorConsts;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.JobMonitoring
{
    public class ClassificationJobMonitorTest
    {
        private readonly IClassificationJobMonitorDataAccess _dataAccess;
        private readonly IIncorrectDataMonitoringService _incorrectDataMonitoringService;
        private readonly ISentryClient _sentryClient;

        private readonly IClassificationJobMonitorService _service;

        public ClassificationJobMonitorTest()
        {
            _dataAccess = Substitute.For<IClassificationJobMonitorDataAccess>();
            _incorrectDataMonitoringService = Substitute.For<IIncorrectDataMonitoringService>();
            _sentryClient = Substitute.For<ISentryClient>();

            _service = new ClassificationJobMonitorService(_dataAccess, _incorrectDataMonitoringService, _sentryClient);
        }

        [Fact]
        public void GetRealtimeCustomerPlaceNewBet_WhenSuccess()
        {
            // Arrange

            // Act
            _dataAccess.GetRealtimeCustomerPlaceNewBet(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(new List<long>());

            var result = _service.GetRealtimeCustomerPlaceNewBet(DateTime.MinValue, DateTime.MaxValue);

            // Assert
            _dataAccess.Received(1).GetRealtimeCustomerPlaceNewBet(Arg.Any<DateTime>(), Arg.Any<DateTime>());

            Assert.NotNull(result);
        }

        [Fact]
        public void ClearRealtimeCustomerPlaceNewBets_WhenSuccess()
        {
            // Arrange

            // Act
            _dataAccess.ClearRealtimeCustomerPlaceNewBets(Arg.Any<DateTime>(), Arg.Any<DateTime>());

            _service.ClearRealtimeCustomerPlaceNewBets(DateTime.MinValue, DateTime.MaxValue);

            // Assert
            _dataAccess.Received(1).ClearRealtimeCustomerPlaceNewBets(Arg.Any<DateTime>(), Arg.Any<DateTime>());
        }

        [Fact]
        public void FilterInvalidCustomerCategory_WhenSuccess()
        {
            // Arrange
            var custCategory = new List<CustomerCategoryMonitorEntity>() {
                new CustomerCategoryMonitorEntity { TypeID = (int)InvalidCategory.Invalid_Inactive },
                new CustomerCategoryMonitorEntity { TypeID = (int)InvalidCategory.Invalid_Null },
            };

            var realtimeInactiveCategoryField = TestExtension.GetPrivateFieldInfo(typeof(ClassificationJobMonitorService), PrivateFieldName.RealtimeInactiveCategory);
            var realtimeNullCategoryField = TestExtension.GetPrivateFieldInfo(typeof(ClassificationJobMonitorService), PrivateFieldName.RealtimeNullCategory);

            // Act
            _service.FilterInvalidCustomerCategory(custCategory);

            var realtimeInactiveCategory = realtimeInactiveCategoryField.GetValue(_service);
            var realtimeNullCategory = realtimeNullCategoryField.GetValue(_service);

            // Assert
            Assert.NotNull(realtimeInactiveCategory);
            Assert.NotNull(realtimeNullCategory);
        }

        [Fact]
        public void InsertInvalidCategory_WhenSuccess()
        {
            // Arrange
            var realtimeInactiveCategory = new List<CustomerCategoryMonitorEntity>() {
                new CustomerCategoryMonitorEntity { TypeID = (int)InvalidCategory.Invalid_Inactive },
            };

            var realtimeNullCategory = new List<CustomerCategoryMonitorEntity>() {
                new CustomerCategoryMonitorEntity { TypeID = (int)InvalidCategory.Invalid_Null },
            };

            var realtimeInactiveCategoryField = TestExtension.GetPrivateFieldInfo(typeof(ClassificationJobMonitorService), PrivateFieldName.RealtimeInactiveCategory);
            realtimeInactiveCategoryField.SetValue(_service, realtimeInactiveCategory);

            var realtimeNullCategoryField = TestExtension.GetPrivateFieldInfo(typeof(ClassificationJobMonitorService), PrivateFieldName.RealtimeNullCategory);
            realtimeNullCategoryField.SetValue(_service, realtimeNullCategory);

            // Act
            _incorrectDataMonitoringService.InsertIncorrectDataMonitoring(Arg.Any<IEnumerable<long>>(), Arg.Any<JobMonitor_IssueType>(), Arg.Any<int>());

            _service.InsertInvalidCategory(500);

            // Assert
            _incorrectDataMonitoringService.Received(2).InsertIncorrectDataMonitoring(Arg.Any<IEnumerable<long>>(), Arg.Any<JobMonitor_IssueType>(), Arg.Any<int>());
        }

        [Fact]
        public void GetCustomerCategoryToMonitor_WhenSuccess()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };
            var batchSize = 1;

            // Act
            _dataAccess.GetCustomerCategoryToMonitor(Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<CustomerCategoryMonitorEntity> { new CustomerCategoryMonitorEntity() });

            var result = _service.GetCustomerCategoryToMonitor(custIds, batchSize);

            // Assert
            _dataAccess.Received(5).GetCustomerCategoryToMonitor(Arg.Any<string>(), Arg.Any<bool>());

            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetNormalClassificationPoolMonitor_WhenSuccess()
        {
            // Arrange

            // Act
            _dataAccess.GetNormalClassificationPoolMonitor().Returns(new NormalClassificationPoolMonitorEntity());

            var result = _service.GetNormalClassificationPoolMonitor();

            // Assert
            _dataAccess.Received(1).GetNormalClassificationPoolMonitor();

            Assert.NotNull(result);
        }

        [Fact]
        public void NotifyExcessiveClassificationPool_WhenSuccess()
        {
            // Arrange

            // Act
            _incorrectDataMonitoringService.InsertIncorrectDataMonitoring(Arg.Any<IEnumerable<InsertMonitoringEntity>>(), Arg.Any<JobMonitor_IssueType>(), Arg.Any<int>());

            _service.NotifyExcessiveClassificationPool(999, true, 999, 999);

            // Assert
            _incorrectDataMonitoringService.Received(1).InsertIncorrectDataMonitoring(Arg.Any<IEnumerable<InsertMonitoringEntity>>(), Arg.Any<JobMonitor_IssueType>(), Arg.Any<int>());
        }
    }
}
