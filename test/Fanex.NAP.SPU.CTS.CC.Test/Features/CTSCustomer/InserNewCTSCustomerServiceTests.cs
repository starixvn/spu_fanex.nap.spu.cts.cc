namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class InserNewCTSCustomerServiceTests
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ISentryClient sentryClient;

        public InserNewCTSCustomerServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, rescanClassificationService);
        }

        [Fact]
        public void TransformCustomerFromMainDB_WhenDataHasPropertyNull1_RunSuccessfully()
        {
            var expectedResult = new List<InsertCustomerModel>();

            var inputData = new List<CustomerMainDBModel>();
            dataAccess.TransformCustomerFromMainDB(string.Empty).Returns(expectedResult);
            var result = ctsCustomerService.TransformCustomerFromMainDB(inputData);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void InsertNewCustomer_WhenDataHasPropertyNull_RunSuccessfully()
        {
            var data = new UpdateCustomerModel
            {
                CustomerInfos = null,
                LastInsertCustID = 123
            };
            var result = ctsCustomerService.InsertNewCustomer(data, false, 100, string.Empty);
            result.Should().BeFalse();
        }

        [Fact]
        public void InsertNewCustomer_WhenNullData_RunSuccessfully()
        {
            var result = ctsCustomerService.InsertNewCustomer(null, false, 100, string.Empty);
            result.Should().BeFalse();
        }

        [Fact]
        public void GetCustSubFromMainDb_WhenSuccess_ReturnData()
        {
            var dataFromDB = new List<CustomerMainDBModel>()
            {
                CreateSampleCustomerMainDBModel()
            };

            dataAccess.GetCustSubFromMainDB(100,1).Returns(dataFromDB);

            var realData = ctsCustomerService.GetCustSubFromMainDB(100, 1);
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
            var dataFromDB = new UpdateCustomerModel()
            {
                CustomerInfos = new List<CustomerMainDBModel>
                {
                    CreateSampleCustomerMainDBModel()
                },
                LastInsertCustID = 1111
            };

            dataAccess.GetCustomerFromMainDBForInsertNew(100, 1).Returns(dataFromDB);

            var realData = ctsCustomerService.GetCustomerFromMainDBForInsertNew(100, 1);
            realData.Should().BeEquivalentTo(dataFromDB);
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