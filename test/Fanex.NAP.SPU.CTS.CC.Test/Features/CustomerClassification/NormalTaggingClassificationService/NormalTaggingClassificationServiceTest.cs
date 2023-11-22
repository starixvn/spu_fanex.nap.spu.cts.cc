namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalTaggingClassificationService
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NSubstitute.ReceivedExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
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
            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning();

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();
        }

        [Fact]
        public void GetNormalTaggingScanning_ReturnException()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Throws<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning();

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();
        }

        [Fact]
        public void GetNormalTaggingScanning_ReturnNotException()
        {
            // Arrange
            var custInfos = new List<GetNormalTaggingScanningViewModel> {
                new GetNormalTaggingScanningViewModel { CustID = 1, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 2, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 3, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 4, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 5, IsCheckTaggingTW = true },
            };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustInfos = custInfos };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(_getNormalTaggingScanningModel);

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning();

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();
        }
        #endregion

        #region PreprocessCustomerClassNormalTagging
        [Fact]
        public void PreprocessCustomerClassNormalTagging_WhenSuccess()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(new GetNormalTaggingScanningModel());

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1000);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalTaggingClassificationDataAccess.Received(0).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCustomerClassNormalTagging_ReturnNotException()
        {
            // Arrange
            var custInfos = new List<GetNormalTaggingScanningViewModel> {
                new GetNormalTaggingScanningViewModel { CustID = 1, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 2, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 3, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 4, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 5, IsCheckTaggingTW = true },
            };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustInfos = custInfos };

            var inserAccountList = new List<InsertNormalAccountModel> { (InsertNormalAccountModel)FormatterServices.GetUninitializedObject(typeof(InsertNormalAccountModel)) };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(_getNormalTaggingScanningModel);

            _normalAccountService.MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(5).Returns(inserAccountList);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<string>()).Received(5).Returns(new List<long> { 1, 2, 3, 4, 5 });

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>()));

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1);

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalAccountService.Received(5).MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalTaggingClassificationDataAccess.Received(5).CheckChangesNormalTaggingScanning(Arg.Any<string>());

            _normalTaggingClassificationDataAccess.Received(5).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCutomerClassNormalTagging_ReturnException()
        {
            // Arrange
            var custInfos = new List<GetNormalTaggingScanningViewModel> {
                new GetNormalTaggingScanningViewModel { CustID = 1, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 2, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 3, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 4, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 5, IsCheckTaggingTW = true },
            };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustInfos = custInfos };

            var insertAccountList = new List<InsertNormalAccountModel> { (InsertNormalAccountModel)FormatterServices.GetUninitializedObject(typeof(InsertNormalAccountModel)) };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(_getNormalTaggingScanningModel);

            _normalAccountService.MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1).Returns(insertAccountList);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<string>()).Returns(new List<long> { 1, 2, 3, 4, 5 });

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1);

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalAccountService.Received(1).MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalTaggingClassificationDataAccess.Received(1).CheckChangesNormalTaggingScanning(Arg.Any<string>());

            _normalTaggingClassificationDataAccess.Received(1).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }

        [Fact]
        public void PreprocessCutomerClassNormalTagging_ReturnGetNormalTaggingException()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Throws<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1);

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalTaggingClassificationDataAccess.Received(0).PreprocessCustomerClassNormalTagging(Arg.Any<string>());
        }
        #endregion

        #region UpdateLastScanningNormalTaggingParameter
        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_WhenSuccess()
        {
            // Arrange

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(new GetNormalTaggingScanningModel());

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1000).UpdateLastScanningNormalTaggingParameter();

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalTaggingClassificationDataAccess.Received(0).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(2).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }

        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_ReturnNotException()
        {
            // Arrange
            var custIds = new List<GetNormalTaggingScanningViewModel> {
                new GetNormalTaggingScanningViewModel { CustID = 1, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 2, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 3, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 4, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 5, IsCheckTaggingTW = true },
            };

            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustInfos = custIds };

            var insertAccountList = new List<InsertNormalAccountModel> { (InsertNormalAccountModel)FormatterServices.GetUninitializedObject(typeof(InsertNormalAccountModel)) };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(_getNormalTaggingScanningModel);

            _normalAccountService.MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(5).Returns(insertAccountList);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<string>()).Received(5).Returns(new List<long> { 1, 2, 3, 4, 5 });

            _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1).UpdateLastScanningNormalTaggingParameter();

            // Assert
            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalAccountService.Received(5).MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalTaggingClassificationDataAccess.Received(5).CheckChangesNormalTaggingScanning(Arg.Any<string>());

            _normalTaggingClassificationDataAccess.Received(5).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(2).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }

        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_ReturnException()
        {
            // Arrange
            var custInfos = new List<GetNormalTaggingScanningViewModel> {
                new GetNormalTaggingScanningViewModel { CustID = 1, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 2, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 3, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 4, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 5, IsCheckTaggingTW = true },
            };
            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustInfos = custInfos };

            var insertAccountList = new List<InsertNormalAccountModel> { (InsertNormalAccountModel)FormatterServices.GetUninitializedObject(typeof(InsertNormalAccountModel)) };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(_getNormalTaggingScanningModel);

            _normalAccountService.MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1).Returns(insertAccountList);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<string>()).Returns(new List<long> { 1, 2, 3, 4, 5 });

            _systemParameterDataAccess.When(x => x.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>())).Throw<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1).UpdateLastScanningNormalTaggingParameter();

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalAccountService.Received(5).MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalTaggingClassificationDataAccess.Received(5).CheckChangesNormalTaggingScanning(Arg.Any<string>());

            _normalTaggingClassificationDataAccess.Received(5).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(1).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }

        [Fact]
        public void UpdateLastScanningNormalTaggingParameter_ReturnPreprocessException()
        {
            // Arrange
            var custInfos = new List<GetNormalTaggingScanningViewModel> {
                new GetNormalTaggingScanningViewModel { CustID = 1, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 2, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 3, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 4, IsCheckTaggingTW = true },
                new GetNormalTaggingScanningViewModel { CustID = 5, IsCheckTaggingTW = true },
            };
            var _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel { CustInfos = custInfos };

            var insertAccountList = new List<InsertNormalAccountModel> { (InsertNormalAccountModel)FormatterServices.GetUninitializedObject(typeof(InsertNormalAccountModel)) };

            // Act
            _normalTaggingClassificationDataAccess.GetNormalTaggingScanning().Returns(_getNormalTaggingScanningModel);

            _normalAccountService.MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>()).Received(1).Returns(insertAccountList);

            _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(Arg.Any<string>()).Returns(new List<long> { 1, 2, 3, 4, 5 });

            _normalTaggingClassificationDataAccess.When(x => x.PreprocessCustomerClassNormalTagging(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _normalTaggingClassificationServices.Start().GetNormalTaggingScanning().PreprocessCustomerClassNormalTagging(1).UpdateLastScanningNormalTaggingParameter();

            // Assert
            action.Should().Throw<Exception>();

            _normalTaggingClassificationDataAccess.Received(1).GetNormalTaggingScanning();

            _normalAccountService.Received(1).MergeWithTaggedNormalTaggingFlow(Arg.Any<IEnumerable<GetNormalAccountEntity>>());

            _normalTaggingClassificationDataAccess.Received(1).CheckChangesNormalTaggingScanning(Arg.Any<string>());

            _normalTaggingClassificationDataAccess.Received(1).PreprocessCustomerClassNormalTagging(Arg.Any<string>());

            _systemParameterDataAccess.Received(0).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<Shares.Constants.SystemParameters>());
        }
        #endregion
    }
}