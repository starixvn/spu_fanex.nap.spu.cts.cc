namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using Newtonsoft.Json;
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
        private readonly ICustomerCategoryService customerCategoryService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        public InserNewCTSCustomerServiceTests()
        {
            dataAccess = Substitute.For<ICTSCustomerDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();
            ctsCustomerService = new CTSCustomerService(dataAccess, sentryClient, rescanClassificationService, customerCategoryService, custClassToMainDBService, antiFraudCenterServices, systemParameterDataAccess);
        }

        [Fact]
        public void TransformCustomerFromMainDB_WhenDataHasPropertyNull1_RunSuccessfully()
        {
            var expectedResult = new CustInsertedInfoModel();

            var inputData = new List<CustomerMainDBModel>();
            dataAccess.TransformCustomerFromMainDB(string.Empty).Returns(expectedResult);
            var result = ctsCustomerService.TransformCustomerFromMainDB(inputData);

            result.Should().BeNull();
        }

        [Fact]
        public void GetLastScannTime_WhenSuccess_RunSuccessfully()
        {
            var result = ctsCustomerService.GetLastScanTimeForCustomerMonitor(Shares.Constants.SystemParameters.InsertNewCustomerMonitoring);
            result.Should().NotBe(DateTime.MinValue);
        }

        [Fact]
        public void GetCustInfoFromMainDB_WhenSuccess_RunSuccessfully()
        {
            var fromDate = new DateTime(2021, 1, 1);
            var toDate = new DateTime(2022, 1, 1);
            var expectedResult = new List<CustomerMainDBModel> { CreateSampleCustomerMainDBModel() };

            dataAccess.GetCustInfoFromMainDBForMonitor(fromDate, toDate).Returns(expectedResult);
            var result = ctsCustomerService.GetCustInfoFromMainDBForMonitor(fromDate, toDate);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void GetCustSubInfoFromMainDB_WhenSuccess_RunSuccessfully()
        {
            var fromDate = new DateTime(2021, 1, 1);
            var toDate = new DateTime(2022, 1, 1);
            var expectedResult = new List<CustomerMainDBModel> { CreateSampleCustomerMainDBModel() };

            dataAccess.GetCustInfoFromMainDBForMonitor(fromDate, toDate).Returns(expectedResult);
            var result = ctsCustomerService.GetCustInfoFromMainDBForMonitor(fromDate, toDate);

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void CompareCustomerDataForMonitor_WhenSuccess_RunSuccessfully()
        {
            var inputData = new List<CustomerMainDBModel>();
            var expectedResult = new List<CustomerMainDBModel> { CreateSampleCustomerMainDBModel() };

            var customerJson = JsonConvert.SerializeObject(inputData);
            dataAccess.CompareCustomerDataForMonitor(customerJson).Returns(expectedResult);
            var result = ctsCustomerService.CompareCustomerDataForMonitor(inputData);

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