namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class UpdateCTSCustomerServiceTests
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ISentryClient sentryClient;

        public UpdateCTSCustomerServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, rescanClassificationService);
        }

        [Fact]
        public void GetLastUpdatedTime_WhenSuccess_ReturnData()
        {
            var expectedResult = new LastUpdateValueModel
            {
                LastUpdateCustIDCustInfo = 1,
                LastUpdateCustIDCustomer = 1,
                LastUpdateTimeCustInfo = DateTime.Now,
                LastUpdateTimeCustomer = DateTime.Now
            };

            dataAccess.GetLastUpdatedTime().Returns(expectedResult);
            var result = ctsCustomerService.GetLastUpdatedTime();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetUpdatedCustFromCustInfoHistory_WhenSuccess_ReturnData()
        {
            var expectedResult = new TranformCustModel
            {
                CustIdList = "4,5,6",
                LastCustId = 3,
                LastUpdatedTime = DateTime.Now.AddMinutes(-3)
            };
            var inputDateTime = DateTime.Now;
            dataAccess.GetUpdatedCustFromCustInfoHistory(inputDateTime, 1, 100).Returns(expectedResult);
            var result = ctsCustomerService.GetUpdatedCustFromCustInfoHistory(inputDateTime, 1, 100);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetUpdatedCustFromCustomerHistory_WhenSuccess_ReturnData()
        {
            var expectedResult = new TranformCustModel
            {
                CustIdList = "4,5,6",
                LastCustId = 3,
                LastUpdatedTime = DateTime.Now.AddMinutes(-3)
            };
            var inputDateTime = DateTime.Now;
            dataAccess.GetUpdatedCustFromCustomerHistory(inputDateTime, 1, 100).Returns(expectedResult);
            var result = ctsCustomerService.GetUpdatedCustFromCustomerHistory(inputDateTime, 1, 100);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetCustomerFromMainDBForUpdate_WhenSuccess_ReturnData()
        {
            var expectedResult = new List<CustomerMainDBModel>
            {
                CreateSampleCustomerMainDBModel()
            };

            dataAccess.GetCustomerFromMainDBForUpdate("1,2,3", UpdateCustomer.FromCustInfo).Returns(expectedResult);
            var result = ctsCustomerService.GetCustomerFromMainDBForUpdate("1,2,3", UpdateCustomer.FromCustInfo);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetCustomerFromCTSForUpdate_WhenSuccess_ReturnData()
        {
            var expectedResult = new List<CustomerMainDBModel>
            {
                CreateSampleCustomerMainDBModel()
            };

            dataAccess.GetCustomerFromCTSForUpdate("1,2,3", UpdateCustomer.FromCustInfo).Returns(expectedResult);
            var result = ctsCustomerService.GetCustomerFromCTSForUpdate("1,2,3", UpdateCustomer.FromCustInfo);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void CompareCustomerInfo_WhenSuccess_ReturnData()
        {
            var custFromCTS = new List<CustomerMainDBModel>
            {
                new CustomerMainDBModel
                {
                    BatchID = 1,
                    CreatedDate = DateTime.Now,
                    Currency = "RM",
                    CurrencyID = 1,
                    CustID = 1,
                    CustStatusID = 1,
                    CustSubID = 1,
                    Danger1 = 1,
                    Danger2 = 1,
                    Danger3 = 1,
                    MRecommend = 1,
                    Recommend = 1,
                    RoleID = 1,
                    Site = "789y",
                    SiteID = 12,
                    SRecommend = 444,
                    UserName = "harbet789",
                    UserName2 = "superrich"
                }
            };

            var custFromMain = new List<CustomerMainDBModel>
            {
                new CustomerMainDBModel
                {
                    BatchID = 1,
                    CreatedDate = DateTime.Now,
                    Currency = "RM",
                    CurrencyID = 1,
                    CustID = 1,
                    CustStatusID = 1,
                    CustSubID = 1,
                    Danger1 = 10,
                    Danger2 = 1,
                    Danger3 = 1,
                    MRecommend = 1,
                    Recommend = 1,
                    RoleID = 1,
                    Site = "789y",
                    SiteID = 12,
                    SRecommend = 444,
                    UserName = "harbet789",
                    UserName2 = "superrich"
                }
            };

            var expectedData = new List<CustInfoModel>
            { 
                new CustInfoModel
                {
                    CustID = custFromMain[0].CustID,
                    CustStatusID = custFromMain[0].CustStatusID,
                    Danger1 = custFromMain[0].Danger1,
                    Danger2 = custFromMain[0].Danger2,
                    Danger3 = custFromMain[0].Danger3
                } 
            };

            var expectedResult = JsonConvert.SerializeObject(expectedData);
            var result = ctsCustomerService.CompareCustomerInfo(custFromMain, custFromCTS, UpdateCustomer.FromCustInfo);

            result.Should().BeEquivalentTo(expectedResult);
        }

        private CustomerMainDBModel CreateSampleCustomerMainDBModel()
        {
            var rd = new Random();
            var data = new CustomerMainDBModel
            {
                BatchID = rd.Next(int.MinValue, int.MaxValue),
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
    }
}