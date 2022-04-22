namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerCategory
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;    
    using System.Collections.Generic;
    using Xunit;

    public class CustomerCategoryServiceTest
    {
        private readonly ICustomerCategoryService customerCategoryService;
        private readonly ICustomerCategoryDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public CustomerCategoryServiceTest()
        {
            dataAccess = Substitute.For<ICustomerCategoryDataAccess>();
            sentryClient = Substitute.For<ISentryClient>();
            customerCategoryService = new CustomerCategoryService(dataAccess, sentryClient);
        }

        [Fact]
        public void GetCustomerCategoryQueue_WhenSuccess_ReturnData()
        {
            var expectedResult = new CustomerCategoryQueueModel
            {
                CustomerCategoryQueue = new List<CustomerCategoryEntity>(),
                QueueInfo = new QueueInfoEntity()
            };

            dataAccess.GetCustomerCategoryQueue().Returns(expectedResult);
            var result = customerCategoryService.GetCustomerCategoryQueue();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void AddCustomerCategory_WhenSuccess_ReturnData()
        {
            var model = new CustomerCategoryQueueModel();
            var custClassifications = new List<LatestClassificationEntity>();

            dataAccess.AddCustomerCategory(model).Returns(custClassifications);

            var result = customerCategoryService.AddCustomerCategory(model);

            result.Should().BeEquivalentTo(custClassifications);
        }

        [Fact]
        public void AddCustomerCategory_2_WhenSuccess_ReturnData()
        {
            var model = new List<CustInfoVVipModel>();
            var custClassifications = new List<LatestClassificationEntity>();

            dataAccess.AddCustomerCategory(model).Returns(custClassifications);

            var result = customerCategoryService.AddCustomerCategory(model);

            result.Should().BeEquivalentTo(custClassifications);
        }

        [Fact]
        public void RemoveCustomerCategory_WhenSuccess_ReturnData()
        {
            var model = new CustomerCategoryQueueModel();
            var custClassifications = new List<LatestClassificationEntity>();

            dataAccess.AddCustomerCategory(model).Returns(custClassifications);

            var result = customerCategoryService.AddCustomerCategory(model);

            result.Should().BeEquivalentTo(custClassifications);
        }
    }
}
