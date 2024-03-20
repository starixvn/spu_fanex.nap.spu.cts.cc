namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.Transform.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.MonitorJobs.CustomerJobMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class InserNewCTSCustomerServiceTests
    {
        private readonly ICustomerJobMonitorService ctsCustomerService;
        private readonly ICustomerJobMonitorDataAccess customerAccess;
        private readonly IClassificationJobMonitorDataAccess classificationAccess;

        public InserNewCTSCustomerServiceTests()
        {
            customerAccess = Substitute.For<ICustomerJobMonitorDataAccess>();
            classificationAccess = Substitute.For<IClassificationJobMonitorDataAccess>();
            ctsCustomerService = new CustomerJobMonitorService(customerAccess, classificationAccess);
        }

        //[Fact] /TODO
        //public void TransformCustomerFromMainDB_WhenDataHasPropertyNull1_RunSuccessfully()
        //{
        //    var expectedResult = new InsertNewCustomerResultViewModel();

        //    var inputData = new List<CustomerMainDBModel>();
        //    dataAccess.TransformCustomerFromMainDB(string.Empty).Returns(expectedResult);
        //    var result = ctsCustomerService.InsertNewCTSCustomer(inputData);

        //    result.Should().BeNull();
        //}

        //[Fact]
        //public void GetCustInfoFromMainDB_WhenSuccess_RunSuccessfully()
        //{
        //    var fromDate = new DateTime(2021, 1, 1);
        //    var toDate = new DateTime(2022, 1, 1);
        //    var expectedResult = new List<CustomerMainDBModel> { CreateSampleCustomerMainDBModel() };

        //    dataAccess.GetCustInfoFromMainDBForMonitor(fromDate, toDate).Returns(expectedResult);
        //    var result = ctsCustomerService.GetCustInfoFromMainDBForMonitor(fromDate, toDate);

        //    result.Should().BeEquivalentTo(expectedResult);
        //}

        //[Fact]
        //public void GetCustSubInfoFromMainDB_WhenSuccess_RunSuccessfully()
        //{
        //    var fromDate = new DateTime(2021, 1, 1);
        //    var toDate = new DateTime(2022, 1, 1);
        //    var expectedResult = new List<CustomerMainDBModel> { CreateSampleCustomerMainDBModel() };

        //    dataAccess.GetCustInfoFromMainDBForMonitor(fromDate, toDate).Returns(expectedResult);
        //    var result = ctsCustomerService.GetCustInfoFromMainDBForMonitor(fromDate, toDate);

        //    result.Should().BeEquivalentTo(expectedResult);
        //}

        [Fact]
        public void CompareCustomerDataForMonitor_WhenSuccess_RunSuccessfully()
        {
            var inputData = new List<MonitoringNewCustomerEntity>() { new MonitoringNewCustomerEntity() };
            var expectedResult = new List<MissingCustomerCompareEntity> { CreateSampleCustomerCompareModel() };

            var customerJson = JsonConvert.SerializeObject(inputData);
            customerAccess.CheckMissingCustomer(customerJson).Returns(expectedResult);
            var result = ctsCustomerService.CheckMissingCustomer(inputData);

            result.Should().BeEquivalentTo(expectedResult);
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

        private MissingCustomerCompareEntity CreateSampleCustomerCompareModel()
        {
            var rd = new Random();
            var data = new MissingCustomerCompareEntity
            {
                CustID = rd.Next(int.MinValue, int.MaxValue),
                CustSubID = rd.Next(int.MinValue, int.MaxValue),             
                UserName = "harbet789",

            };

            return data;
        }
    }
}