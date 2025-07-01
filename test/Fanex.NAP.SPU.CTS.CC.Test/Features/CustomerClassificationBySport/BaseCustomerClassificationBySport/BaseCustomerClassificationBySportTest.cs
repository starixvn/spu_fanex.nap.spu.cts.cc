using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
using Fanex.NAP.SPU.CTS.Shares;
using NSubstitute;
using Xunit;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport
{
    public class BaseCustomerClassificationBySportTest
    {
        private readonly IBaseCustomerClassificationBySportDataAccess _baseDataAccess;

        private readonly IBaseCustomerClassificationBySportService _service;

        public BaseCustomerClassificationBySportTest()
        {
            _baseDataAccess = Substitute.For<IBaseCustomerClassificationBySportDataAccess>();

            _service = new BaseCustomerClassificationBySportService(_baseDataAccess);
            LogExtension.ConfigByPassForUnitTest();
        }

        [Fact]
        public void InsertCustomerClassificationBySport_WhenSuccess()
        {
            // Arrange
            var data = new InsertCustomerClassificationBySportModel();

            // Act
            _baseDataAccess
                .InsertCustomerClassificationBySport(CCInputFlow.Normal, data)
                .Returns(new ResultInsertClassificationBySportModel());

            var result = _service.InsertCustomerClassificationBySport(CCInputFlow.Normal, data, string.Empty);

            // Assert
            _baseDataAccess.Received(1).InsertCustomerClassificationBySport(Arg.Any<CCInputFlow>(), data);

            Assert.NotNull(result);
        }

        [Fact]
        public void InsertCustomerClassificationBySport_WhenFailure()
        {
            // Arrange
            var data = new InsertCustomerClassificationBySportModel();

            // Act
            _baseDataAccess
                .InsertCustomerClassificationBySport(Arg.Any<CCInputFlow>(), data)
                .Returns(new ResultInsertClassificationBySportModel() { ErrorMessage = "TEST" });

            var result = _service.InsertCustomerClassificationBySport(CCInputFlow.BySportInsertNormal, data, "TEST");

            // Assert
            _baseDataAccess.Received(1).InsertCustomerClassificationBySport(Arg.Any<CCInputFlow>(), data);

            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveCustomerClassificationBySport_WhenSuccess()
        {
            // Arrange

            // Act
            _baseDataAccess
                .RemoveCustomerClassificationBySport(CCInputFlow.RemovePACategory, Arg.Any<RemoveCustomerClassificationBySportModel>())
                .Returns(new ResultRemoveCustomerClassificationBySportModel());

            var result = _service.RemoveCustomerClassificationBySport(CCInputFlow.RemovePACategory, new RemoveCustomerClassificationBySportModel(), string.Empty);

            // Assert
            _baseDataAccess.Received(1).RemoveCustomerClassificationBySport(Arg.Any<CCInputFlow>(), Arg.Any<RemoveCustomerClassificationBySportModel>());

            Assert.NotNull(result);
        }

        [Fact]
        public void RemoveCustomerClassificationBySport_WhenFailure()
        {
            // Arrange
            var data = new RemoveCustomerClassificationBySportModel();

            // Act
            _baseDataAccess
                .RemoveCustomerClassificationBySport(CCInputFlow.RemovePACategory, data)
                .Returns(new ResultRemoveCustomerClassificationBySportModel() { ErrorMessage = "TEST" });

            var result = _service.RemoveCustomerClassificationBySport(CCInputFlow.RemovePACategory, data, string.Empty);

            // Assert
            _baseDataAccess.Received(1).RemoveCustomerClassificationBySport(Arg.Any<CCInputFlow>(), data);

            Assert.NotNull(result);
        }
    }
}
