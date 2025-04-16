namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class InsertNewCTSCustomerServiceTest
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;

        public InsertNewCTSCustomerServiceTest()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, custClassToMainDBService);
        }

        [Fact]
        public void GetCustSubFromMainDb_WhenSuccess_ReturnData()
        {
            var dataFromDB = new List<CustomerMainDBEntity>()
            {
                CreateSampleCustomerMainDBModel()
            };

            dataAccess.GetCustSubFromMainDBByLastCustSub(100, 1).Returns(dataFromDB);

            var realData = ctsCustomerService.GetCustSubFromMainDBByLastCustSub(100, 1);
            realData.Should().BeEquivalentTo(dataFromDB);
        }

        [Fact]
        public void GetLastInsertCust_WhenSuccess_ReturnData()
        {
            var dataFromDB = new LastScannedCustModel()
            {
                LastCustID = 1111,
                LastCustSubID = 2222
            };

            dataAccess.GetLastInsertCust().Returns(dataFromDB);

            var realData = ctsCustomerService.GetLastInsertCust();
            realData.Should().BeEquivalentTo(dataFromDB);
        }

        [Fact]
        public void GetCustomerFromMainDBForInsertNew_WhenSuccess_ReturnData()
        {
            var dataFromDB = new InsertCustomerModel()
            {
                CustomerInfos = new List<CustomerMainDBEntity>
                {
                    CreateSampleCustomerMainDBModel()
                },
                LastInsertCustID = 1111
            };

            dataAccess.GetCustomerFromMainDBForInsertNew(100, 1).Returns(dataFromDB);

            var realData = ctsCustomerService.GetCustomerFromMainDBForInsertNew(100, 1);
            realData.Should().BeEquivalentTo(dataFromDB);
        }

        private CustomerMainDBEntity CreateSampleCustomerMainDBModel()
        {
            var rd = new Random();
            var data = new CustomerMainDBEntity
            {
                CreatedDate = DateTime.Now,
                Currency = "RM",
                CurrencyID = rd.Next(int.MinValue, int.MaxValue),
                CustID = rd.Next(int.MinValue, int.MaxValue),
                CustStatusID = rd.Next(int.MinValue, int.MaxValue),
                CustSubID = rd.Next(int.MinValue, int.MaxValue),
                Danger1 = rd.Next(int.MinValue, int.MaxValue),
                Danger2 = rd.Next(int.MinValue, int.MaxValue),
                Danger3 = rd.Next(int.MinValue, int.MaxValue),
                MRecommend = rd.Next(int.MinValue, int.MaxValue),
                Recommend = rd.Next(int.MinValue, int.MaxValue),
                RoleID = rd.Next(int.MinValue, int.MaxValue),
                Site = "789y",
                SiteID = 12,
                SRecommend = 444,
                UserName = "harbet789",
                UserName2 = "superrich"
            };

            return data;
        }

        [Fact]
        public void InsertMissingCustomerLatencyToQueue_WhenSuccess_WithNotEmpty()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };

            // Act
            ctsCustomerService.InsertMissingCustomerLatencyToQueue(data);

            // Assert
            dataAccess.Received(1).InsertMissingCustomerLatencyToQueue(Arg.Any<string>());
        }

        [Fact]
        public void InsertMissingCustomerLatencyToQueue_WhenSuccess_WithEmpty()
        {
            // Arrange
            var data = new List<int> { };

            // Act
            ctsCustomerService.InsertMissingCustomerLatencyToQueue(data);

            // Assert
            dataAccess.Received(0).InsertMissingCustomerLatencyToQueue(Arg.Any<string>());
        }
    }
}