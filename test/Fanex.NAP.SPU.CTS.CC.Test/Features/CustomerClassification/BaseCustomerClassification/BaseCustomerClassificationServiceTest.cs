namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.BaseCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using FluentAssertions;
    using NSubstitute;
    using Sentry;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class BaseCustomerClassificationServiceTest
    {
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;
        private readonly IBaseCustomerClassificationService baseService;
        private readonly ISentryClient sentryClient;

        public BaseCustomerClassificationServiceTest()
        {
            sentryClient = Substitute.For<ISentryClient>();
            baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            baseService = new BaseCustomerClassificationService(baseDataAccess, sentryClient);
        }

        [Fact]
        public void GetRescanCustomerClassification_WhenSuccess_ReturnData()
        {
            // Arrange
            var custIDs = "1,2";
            var expectedData = new GetRescanCustomerClassificationModel();

            // Act
            baseDataAccess.GetRescanCustomerClassification(custIDs).Returns(expectedData);

            var result = baseService.GetRescanCustomerClassification(custIDs);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void InsertCustomerClassification_WhenSuccess_ReturnData()
        {
            // Arrange
            var inputFlow = CCInputFlow.InsertPACategory;
            var modelInsert = new InsertCustomerClassificationModel();
            var serviceName = "Test";
            var expectedData = new ResultInsertCustomerClassificationModel();

            // Act
            baseDataAccess.InsertCustomerClassification(inputFlow, modelInsert).Returns(expectedData);

            var result = baseService.InsertCustomerClassification(inputFlow, modelInsert, serviceName);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void InsertCustomerClassification_WhenFail_ReturnErrorMsg()
        {
            // Arrange
            var inputFlow = CCInputFlow.InsertPACategory;
            var modelInsert = new InsertCustomerClassificationModel();
            var serviceName = "Test";
            var expectedData = new ResultInsertCustomerClassificationModel() { CustInfos = null, ErrorMessage = "Fail"};

            // Act
            baseDataAccess.InsertCustomerClassification(inputFlow, modelInsert).Returns(expectedData);

            var result = baseService.InsertCustomerClassification(inputFlow, modelInsert, serviceName);

            // Assert
            result.ErrorMessage.Equals(expectedData.ErrorMessage);
        }

        [Fact]
        public void RemoveCustomerClassification_WhenSuccess_ReturnData()
        {
            // Arrange
            var inputFlow = CCInputFlow.InsertPACategory;
            var modelRemove = new RemoveCustomerClassificationModel();
            var serviceName = "Test";
            var expectedData = new ResultRemoveCustomerClassificationModel();

            // Act
            baseDataAccess.RemoveCustomerClassification(inputFlow, modelRemove).Returns(expectedData);

            var result = baseService.RemoveCustomerClassification(inputFlow, modelRemove, serviceName);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void RemoveCustomerClassification_WhenFail_ReturnErrorMsg()
        {
            // Arrange
            var inputFlow = CCInputFlow.InsertPACategory;
            var modelRemove = new RemoveCustomerClassificationModel();
            var serviceName = "Test";
            var expectedData = new ResultRemoveCustomerClassificationModel (){ CustInfos = null, ErrorMessage = "Fail" };

            // Act
            baseDataAccess.RemoveCustomerClassification(inputFlow, modelRemove).Returns(expectedData);

            var result = baseService.RemoveCustomerClassification(inputFlow, modelRemove, serviceName);

            // Assert
            result.ErrorMessage.Equals(expectedData.ErrorMessage);
        }

        [Fact]
        public void GetCategoryByCategoryTypeName_WhenSuccess_ReturnData()
        {
            // Arrange
            var categoryTypeName = "Test";

            // Act
            baseDataAccess.GetCategoryByCategoryTypeName(categoryTypeName).Returns(1);

            var result = baseService.GetCategoryByCategoryTypeName(categoryTypeName);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
