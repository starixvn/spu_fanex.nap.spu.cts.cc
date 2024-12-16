namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ReactivedCTSCustomerServiceTests
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public ReactivedCTSCustomerServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, null);
        }

        [Fact]
        public void GetReactivedCustomer_WhenSuccess_ReturnData()
        {
            var dataFromDB = new List<ReactivatedCustModel>()
            {
                CreateSampleReactivatedCustModel(),
                CreateSampleReactivatedCustModel()
            };

            var data = DateTime.Now;

            dataAccess.GetReactivatedCustomers(data, null).Returns(dataFromDB);

            var realData = ctsCustomerService.GetReactivatedCustomers(data, null);
            realData.Should().BeEquivalentTo(dataFromDB);
        }

        [Fact]
        public void GetCustSubFromMainDB_WhenSuccess_ReturnData()
        {
            var dataFromDB = new List<CustomerMainDBModel>()
            {
                CreateSampleCustomerMainDBModel(),
                CreateSampleCustomerMainDBModel()
            };

            dataAccess.GetCustSubFromMainDBByCustIDs("1,2,3").Returns(dataFromDB);

            var realData = ctsCustomerService.GetCustSubFromMainDBByCustIDs("1,2,3");
            realData.Should().BeEquivalentTo(dataFromDB);
        }

        [Fact]
        public void GetInactivateNewCustomers_WhenSuccess_ReturnData()
        {
            var dataFromDB = new InactivateNewCustomer
            {
                CustInfos = new List<RescanCategoryDailyModel>
                {
                    new RescanCategoryDailyModel { CustID = 1, CurrentCategoryID = 201} ,
                    new RescanCategoryDailyModel { CustID = 2, CurrentCategoryID = 202}
                },
                LastCustID = 1,
                LastCategoryID = 1
            };

            dataAccess.GetInactivateNewCustomers().Returns(dataFromDB);

            var realData = ctsCustomerService.GetInactivateNewCustomers();
            realData.Should().BeEquivalentTo(dataFromDB);
        }

        private CustomerMainDBModel CreateSampleCustomerMainDBModel()
        {
            var rd = new Random();
            var data = new CustomerMainDBModel
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
                UserName = "test",
                UserName2 = "test"
            };

            return data;
        }

        private ReactivatedCustModel CreateSampleReactivatedCustModel()
        {
            var rd = new Random();
            var data = new ReactivatedCustModel
            {
                LogTime = DateTime.Now,
                CustID = rd.Next(int.MinValue, int.MaxValue)
            };

            return data;
        }
    }
}