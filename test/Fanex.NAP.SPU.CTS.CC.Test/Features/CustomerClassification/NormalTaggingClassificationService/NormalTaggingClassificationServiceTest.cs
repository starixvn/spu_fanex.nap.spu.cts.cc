namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalTaggingClassificationService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NSubstitute.ReceivedExtensions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class NormalTaggingClassificationServiceTest
    {
        private readonly INormalTaggingClassificationDataAccess _normalTaggingClassificationDataAccess;
        private readonly INormalTaggingClassificationServices _normalTaggingClassificationServices;

        public NormalTaggingClassificationServiceTest()
        {
            _normalTaggingClassificationDataAccess = Substitute.For<INormalTaggingClassificationDataAccess>();
            _normalTaggingClassificationServices = new NormalTaggingClassificationServices(_normalTaggingClassificationDataAccess);
        }

        [Fact]
        public void GetCustIdsForNormalTaggingScanning_WhenSuccess()
        {
            // Arrange

            // Act
            _normalTaggingClassificationServices.Start().GetCustIdsForNormalTaggingScanning(5000);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetTaggingCustIds(5000);
        }

        [Fact]
        public void GetTaggingCustId_ReturnException()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetTaggingCustIds(5000).Throws<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetCustIdsForNormalTaggingScanning(5000);

            // Assert
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void GetTaggingCustIds_ReturnNotException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            // Act
            _normalTaggingClassificationDataAccess.GetTaggingCustIds(5000).Returns(custIds);

            _normalTaggingClassificationServices.Start().GetCustIdsForNormalTaggingScanning(5000);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetTaggingCustIds(5000);
        }

        [Fact]
        public void PreprocessCustomerClassNormalTagging_WhenSuccess()
        {
            // Arrange
            _normalTaggingClassificationDataAccess.GetTaggingCustIds(5000).Returns(new List<long>());

            // Act
            _normalTaggingClassificationServices.Start().GetCustIdsForNormalTaggingScanning(5000).PreprocessCustomerClassNormalTagging(1000);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetTaggingCustIds(5000);
            _normalTaggingClassificationDataAccess.Received(0).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCustomerClassNormalTagging_ReturnNotException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            // Act
            _normalTaggingClassificationDataAccess.GetTaggingCustIds(5).Returns(custIds);

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>()));

            _normalTaggingClassificationServices.Start().GetCustIdsForNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetTaggingCustIds(5);
            _normalTaggingClassificationDataAccess.Received(5).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCutomerClassNormalTagging_ReturnException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            // Act
            _normalTaggingClassificationDataAccess.GetTaggingCustIds(5).Returns(custIds);

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetCustIdsForNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1);

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetTaggingCustIds(5);
        }
    }
}