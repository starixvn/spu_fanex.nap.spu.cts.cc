namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.DailyClassification;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DailyClassificationTest
    {
        private readonly IDailyClassificationDataAccess dailyClassificationDataAccess;
        private readonly IDailyClassificationServices dailyClassificationServices;

        public DailyClassificationTest()
        {
            dailyClassificationDataAccess = Substitute.For<IDailyClassificationDataAccess>();
            dailyClassificationServices = new DailyClassificationServices(dailyClassificationDataAccess);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(1, -1)]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, 0)]
        public void GetCustClassificationForDailyScanning_WhenSuccess_ReturnCustClassificationList(int batchQuantity, int rowQuantityPerBatch)
        {
            var dailyScanningClassiifications = new List<CustClassificationDailyEntity>
            {
               new CustClassificationDailyEntity
               {
                    CustId = 1,
                    CategoryId = 1,
                    BatchID = 1,
                    SportGroupId = 1,
                    CreatedTime = DateTime.Parse("2021-07-07 22:50:55")
               },
               new CustClassificationDailyEntity
               {
                    CustId = 2,
                    CategoryId = 2,
                    BatchID = 2,
                    SportGroupId = 2,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55")
               }
            };

            dailyClassificationDataAccess.GetCustomerForDailyScanning(batchQuantity, rowQuantityPerBatch).Returns(dailyScanningClassiifications);

            var result = dailyClassificationServices.GetCustClassificationForDailyScanning(batchQuantity, rowQuantityPerBatch);

            result.Should().BeEquivalentTo(dailyScanningClassiifications);
        }

        [Fact]
        public void ClassifyCustomerCategoryByCustIds_WhenSuccess_ReturnNotException()
        {
            var custIds = new List<long> { 1, 2, 3 };

            dailyClassificationDataAccess.ClassifyCustomerGeneralCategoryDaily(string.Join(",", custIds), true);

            Action action = () => dailyClassificationServices.ClassifyCustomerGeneralCategoryDaily(custIds, true);
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void ClassifyCustomerCategory_WhenSuccess_ReturnNotException()
        {
            var dailyScanningClassiifications = new List<CustClassificationDailyEntity>
            {
               new CustClassificationDailyEntity
               {
                    CustId = 1,
                    CategoryId = 1,
                    BatchID = 1,
                    SportGroupId = 1,
                    CreatedTime = DateTime.Parse("2021-07-07 22:50:55")
               },
               new CustClassificationDailyEntity
               {
                    CustId = 2,
                    CategoryId = 2,
                    BatchID = 2,
                    SportGroupId = 2,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55")
               }
            };

            var custIds = string.Join(",", dailyScanningClassiifications.Select(c => c.CustId));

            dailyClassificationDataAccess.ClassifyCustomerGeneralCategoryDaily(string.Join(",", custIds), true);

            Action action = () => dailyClassificationServices.ClassifyCustomerGeneralCategoryDaily(dailyScanningClassiifications, true);
            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateDailyClassificationStatus_WhenSuccess_ReturnNotException()
        {
            var dailyScanningClassiifications = new List<CustClassificationDailyEntity>
            {
               new CustClassificationDailyEntity
               {
                    CustId = 1,
                    CategoryId = 1,
                    BatchID = 1,
                    SportGroupId = 1,
                    CreatedTime = DateTime.Parse("2021-07-07 22:50:55")
               },
               new CustClassificationDailyEntity
               {
                    CustId = 2,
                    CategoryId = 2,
                    BatchID = 2,
                    SportGroupId = 2,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55")
               }
            };

            string jsonData = JsonConvert.SerializeObject(dailyScanningClassiifications.Select(a => new
            {
                a.CustId,
                a.SportGroupId
            }));

            dailyClassificationDataAccess.ClassifyCustomerDailyDone(jsonData);

            Action action = () => dailyClassificationServices.UpdateDailyClassificationStatus(dailyScanningClassiifications);
            action.Should().NotThrow<Exception>();
        }
    }
}