namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalTaggingClassificationService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NSubstitute.ReceivedExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class NormalTaggingClassificationServiceTest
    {
        private readonly INormalTaggingClassificationDataAccess _normalTaggingClassificationDataAccess;
        private readonly INormalTaggingClassificationServices _normalTaggingClassificationServices;
        private readonly INormalAccountService _normalAccountService;
        private readonly ISystemParameterDataAccess _systemParameterDataAccess;

        public NormalTaggingClassificationServiceTest()
        {
            _normalTaggingClassificationDataAccess = Substitute.For<INormalTaggingClassificationDataAccess>();
            _normalAccountService = Substitute.For<INormalAccountService>();
            _systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();
            _normalTaggingClassificationServices = new NormalTaggingClassificationServices(_normalTaggingClassificationDataAccess, _systemParameterDataAccess, _normalAccountService);
        }

        #region GetNormalTaggingScanning
        [Fact]
        public void GetNormalTaggingScanning_WhenSuccess()
        {
            // Arrange

            // Act
            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5000);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5000);
        }

        [Fact]
        public void GetNormalTaggingScanning_ReturnException()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5000).Throws<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5000);

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5000);
        }

        [Fact]
        public void GetNormalTaggingScanning_ReturnNotException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustIDs = custIds };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5000).Returns(_getNormalTaggingScanningModel);

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5000);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5000);
        }
        #endregion

        #region PreprocessCustomerClassNormalTagging
        [Fact]
        public void PreprocessCustomerClassNormalTagging_WhenSuccess()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5000).Returns(new GetNormalTaggingScanningModel());

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5000).PreprocessCustomerClassNormalTagging(1000);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5000);

            _normalTaggingClassificationDataAccess.Received(0).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCustomerClassNormalTagging_ReturnNotException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustIDs = custIds };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5).Returns(_getNormalTaggingScanningModel);

            _normalAccountService.GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1);

            _normalAccountService.MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>()).Received(1);

            _normalAccountService.GetCustomerAssociationProblemAccount(Arg.Any<string>()).Received(1);

            _normalAccountService.MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>()).Received(1);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>()).Returns(custIds);

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>()));

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5);

            _normalAccountService.Received(5).GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalAccountService.Received(5).MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>());

            _normalAccountService.Received(5).GetCustomerAssociationProblemAccount(Arg.Any<string>());

            _normalAccountService.Received(5).MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>());

            _normalTaggingClassificationDataAccess.Received(5).CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>());

            _normalTaggingClassificationDataAccess.Received(5).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCutomerClassNormalTagging_ReturnException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustIDs = custIds };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5).Returns(_getNormalTaggingScanningModel);

            _normalAccountService.GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1);

            _normalAccountService.MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>()).Received(1);

            _normalAccountService.GetCustomerAssociationProblemAccount(Arg.Any<string>()).Received(1);

            _normalAccountService.MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>()).Received(1);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>()).Returns(custIds);

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1);

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5);

            _normalAccountService.Received(1).GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalAccountService.Received(1).MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>());

            _normalAccountService.Received(1).GetCustomerAssociationProblemAccount(Arg.Any<string>());

            _normalAccountService.Received(1).MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>());

            _normalTaggingClassificationDataAccess.Received(1).CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>());

            _normalTaggingClassificationDataAccess.Received(1).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCutomerClassNormalTagging_ReturnGetNormalTaggingException()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5).Throws<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1);

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5);

            _normalTaggingClassificationDataAccess.Received(0).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }
        #endregion

        #region UpdateLastScanningNormalTaggingParameter
        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_WhenSuccess()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5000).Returns(new GetNormalTaggingScanningModel());

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5000).PreprocessCustomerClassNormalTagging(1000).UpdateLastScanningNormalTaggingParameter();

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5000);

            _normalTaggingClassificationDataAccess.Received(0).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(0).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }

        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_ReturnNotException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustIDs = custIds };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5).Returns(_getNormalTaggingScanningModel);

            _normalAccountService.GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1);

            _normalAccountService.MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>()).Received(1);

            _normalAccountService.GetCustomerAssociationProblemAccount(Arg.Any<string>()).Received(1);

            _normalAccountService.MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>()).Received(1);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>()).Returns(custIds);

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1).UpdateLastScanningNormalTaggingParameter();

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5);

            _normalAccountService.Received(5).GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalAccountService.Received(5).MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>());

            _normalAccountService.Received(5).GetCustomerAssociationProblemAccount(Arg.Any<string>());

            _normalAccountService.Received(5).MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>());

            _normalTaggingClassificationDataAccess.Received(5).CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>());

            _normalTaggingClassificationDataAccess.Received(5).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(2).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }

        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_ReturnException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };
            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustIDs = custIds };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5).Returns(_getNormalTaggingScanningModel);

            _normalAccountService.GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1);

            _normalAccountService.MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>()).Received(1);

            _normalAccountService.GetCustomerAssociationProblemAccount(Arg.Any<string>()).Received(1);

            _normalAccountService.MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>()).Received(1);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>()).Returns(custIds);

            _systemParameterDataAccess.When(x => x.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>())).Throw<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1).UpdateLastScanningNormalTaggingParameter();

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5);

            _normalAccountService.Received(5).GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalAccountService.Received(5).MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>());

            _normalAccountService.Received(5).GetCustomerAssociationProblemAccount(Arg.Any<string>());

            _normalAccountService.Received(5).MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>());

            _normalTaggingClassificationDataAccess.Received(5).CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>());

            _normalTaggingClassificationDataAccess.Received(5).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(1).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }

        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_ReturnPreprocessException()
        {
            // Arrange
            var custIds = new List<long> { 1, 2, 3, 4, 5 };
            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustIDs = custIds };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning(5).Returns(_getNormalTaggingScanningModel);

            _normalAccountService.GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1);

            _normalAccountService.MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>()).Received(1);

            _normalAccountService.GetCustomerAssociationProblemAccount(Arg.Any<string>()).Received(1);

            _normalAccountService.MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>()).Received(1);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>()).Returns(custIds);

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning(5).PreprocessCustomerClassNormalTagging(1).UpdateLastScanningNormalTaggingParameter();

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning(5);

            _normalAccountService.Received(1).GetGroupBettingAndRejectCustomer(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalAccountService.Received(1).MergeWithTWTaggedCust(Arg.Any<IEnumerable<GetNormalAccountEntity>>(), Arg.Any<IEnumerable<TWTaggedCustEntity>>());

            _normalAccountService.Received(1).GetCustomerAssociationProblemAccount(Arg.Any<string>());

            _normalAccountService.Received(1).MergeWithCTSTaggedCust(Arg.Any<IEnumerable<NormalAccountAfterCheckTWDetectionModel>>(), Arg.Any<IEnumerable<CTSTaggedCustEntity>>());

            _normalTaggingClassificationDataAccess.Received(1).CheckChangesNormalTaggingScanning(Arg.Any<IEnumerable<CheckChangesNormalTaggingScanningModel>>());

            _normalTaggingClassificationDataAccess.Received(1).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(0).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }
        #endregion
    }
}