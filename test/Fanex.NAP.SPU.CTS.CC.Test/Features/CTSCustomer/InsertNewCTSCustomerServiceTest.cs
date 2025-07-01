namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class InsertNewCTSCustomerServiceTest
    {
        private readonly IInsertNewCustomerDataAccess _dataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;

        private readonly IInsertNewCustomerService _service;

        public InsertNewCTSCustomerServiceTest()
        {
            _dataAccess = Substitute.For<IInsertNewCustomerDataAccess>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            _service = new InsertNewCustomerService(_dataAccess, _custClassToMainDBService);
        }

        [Fact]
        public void GetCustSubFromMainDb_WhenSuccess_ReturnData()
        {
            var dataFromDB = new List<CustomerMainDBEntity>()
            {
                CreateSampleCustomerMainDBModel()
            };

            _dataAccess.GetCustSubFromMainDBByLastCustSub(100, 1).Returns(dataFromDB);

            var realData = _dataAccess.GetCustSubFromMainDBByLastCustSub(100, 1);
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

            _dataAccess.GetLastInsertCust().Returns(dataFromDB);

            var realData = _dataAccess.GetLastInsertCust();
            realData.Should().BeEquivalentTo(dataFromDB);
        }

        [Fact]
        public void GetLastInsertCust_WhenException_ThrowException()
        {
            // Arrange

            // Act
            _dataAccess.GetLastInsertCust().Throws<Exception>();

            Action action = () => _service.GetLastInsertCust();

            // Assert
            Assert.Throws<Exception>(action);
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

            _dataAccess.GetCustomerFromMainDBForInsertNew(100, 1).Returns(dataFromDB);

            var realData = _dataAccess.GetCustomerFromMainDBForInsertNew(100, 1);
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
        public void InsertNewCustomer_WhenSuccess_ReturnData()
        {
            // Arrange
            var data = new InsertCustomerModel
            {
                CustomerInfos = new List<CustomerMainDBEntity>() { new CustomerMainDBEntity { CustID = 1 } },
                LastInsertCustID = 1
            };

            var paras = new InsertNewCustRescanEntity
            {
                InternalBatchSize = 50,
                ExternalBatchSize = 50,
                IsWriteLog = true,
                IsInsertMissingCust = false,
                InsertVIPURL = string.Empty,
                InsertBAURL = string.Empty,
                serviceName = "InsertNewCustomer"
            };

            var expectedData = new List<InsertNewCustomerResultModel>()
            {
                new InsertNewCustomerResultModel
                {
                     CustID = 1, CTSCustID = 1, RoleID = 1
                }
            };

            // Act
            _dataAccess.InsertNewCTSCustomer(Arg.Any<IEnumerable<CustomerMainDBEntity>>(), true).Returns(expectedData);

            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>()).Returns(true);

            var result = _service.InsertNewCustomer(true, data, paras);


            // Assert
            result.Should().BeTrue();
        }
    }
}