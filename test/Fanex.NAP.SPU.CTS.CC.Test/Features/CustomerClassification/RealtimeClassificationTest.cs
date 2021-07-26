namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RealtimeClassification;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class RealtimeClassificationTest
    {
        private readonly IRealtimeClassificationDataAccess realtimeClassificationDataAccess;
        private readonly IRealtimeClassificationServices realtimeClassificationServices;

        public RealtimeClassificationTest()
        {
            realtimeClassificationDataAccess = Substitute.For<IRealtimeClassificationDataAccess>();
            realtimeClassificationServices = new RealtimeClassificationServices(realtimeClassificationDataAccess);
        }

        [Fact]
        public void GetCustHasChangeForRealTime_WhenSuccess_ReturnNotException()
        {
            var realtimeScanningList = new List<CustClassificationRealtimeEntity>
            {
                new CustClassificationRealtimeEntity
                {
                    CustId = 1,
                    CategoryId = 1,
                    BatchID =1,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55"),
                    SportGroupId =1
                },
                new CustClassificationRealtimeEntity
                {
                    CustId = 2,
                    CategoryId = 2,
                    BatchID =2,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55"),
                    SportGroupId =2
                },
            };

            realtimeClassificationDataAccess.GetCustomerGeneralCategoryHasChangeForRunRealtime().Returns(realtimeScanningList);

            realtimeClassificationServices.GetCustomerGeneralCategoryHasChangeForRunRealtime();
            var result = realtimeClassificationServices.CustomerClassification;

            result.Should().BeEquivalentTo(realtimeScanningList);
        }

        [Fact]
        public void GetCustForRunRealtime_WhenSuccess_ReturnCustClassificationList()
        {
            var custClassificationList = new List<CustClassificationRealtimeEntity>
            {
                new CustClassificationRealtimeEntity
                {
                    CustId = 1,
                    CategoryId = 1,
                    BatchID =1,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55"),
                    SportGroupId =1
                },
                new CustClassificationRealtimeEntity
                {
                    CustId = 2,
                    CategoryId = 2,
                    BatchID =2,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55"),
                    SportGroupId =2
                },
            };

            var custSportJson = JsonConvert.SerializeObject(custClassificationList);
            realtimeClassificationDataAccess.GetCustomerGeneralCategoryForRunRealtime(custSportJson).Returns(custClassificationList);

            var result = realtimeClassificationServices.GetCustomerGeneralCategoryForRunRealtime(custClassificationList);

            result.Should().BeEquivalentTo(custClassificationList);
        }

        [Fact]
        public void ClassifyCustomerGeneralCategoryRealtime_WhenSuccess_ReturnNotException()
        {
            var custClassificationList = new List<CustClassificationRealtimeEntity>
            {
                new CustClassificationRealtimeEntity
                {
                    CustId = 1,
                    CategoryId = 1,
                    BatchID =1,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55"),
                    SportGroupId =1
                },
                new CustClassificationRealtimeEntity
                {
                    CustId = 2,
                    CategoryId = 2,
                    BatchID =2,
                    CreatedTime = DateTime.Parse("2021-07-07 12:50:55"),
                    SportGroupId =2
                },
            };

            realtimeClassificationDataAccess.ClassifyCustomerGeneralCategoryRealtime(custClassificationList);

            Action action = () => realtimeClassificationServices.ClassifyCustomerGeneralCategoryRealtime(custClassificationList);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void RealtimeUpdateLastScanTime_WhenSuccess_ReturnNotException()
        {
            realtimeClassificationDataAccess.RealtimeUpdateLastScanTime(true);

            Action action = () => realtimeClassificationServices.RealtimeUpdateLastScanTime(true);

            action.Should().NotThrow<Exception>();
        }
    }
}