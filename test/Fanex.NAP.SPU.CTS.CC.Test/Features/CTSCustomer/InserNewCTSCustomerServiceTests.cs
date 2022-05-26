namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.AffectedCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class MonitorInserNewCustomerServiceTests
    {
        private readonly ICTSCustomerService ctsCustomerService;
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ICustomerCategoryService customerCategoryService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        public MonitorInserNewCustomerServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, rescanClassificationService, customerCategoryService, custClassToMainDBService, antiFraudCenterServices, systemParameterDataAccess);
        }

        //[Fact] TODO
        //public void TransformCustomerFromMainDB_WhenDataHasPropertyNull1_RunSuccessfully()
        //{
        //    var expectedResult = new InsertNewCustomerResultViewModel();

        //    var inputData = new List<CustomerMainDBModel>();
        //    dataAccess.InsertNewCTSCustomer(string.Empty).Returns(expectedResult);
        //    var result = ctsCustomerService.InsertNewCTSCustomer(inputData);

        //    result.Should().BeNull();
        //}

        [Fact]
        public void GetCustSubFromMainDb_WhenSuccess_ReturnData()
        {
            var dataFromDB = new List<CustomerMainDBModel>()
            {
                CreateSampleCustomerMainDBModel()
            };

            dataAccess.GetCustSubFromMainDB(100, 1).Returns(dataFromDB);

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
            var dataFromDB = new InsertCustomerViewModel()
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