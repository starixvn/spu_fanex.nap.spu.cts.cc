using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownline;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownlineCustomerClassification.NewCustomerCustomerClassification.Model;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewCustomer;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RescanNormalClassificationBySport;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.NewCustomerClassificationConsts;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification
{
    public class NewCustomerClassificationTest
    {
        private readonly INewCustomerClassificationDataAccess _newCTSClassificationDataAccess;
        private readonly IAffectedDownlineCustomerClassificationService _affectedCustomerClassificationService;
        private readonly IProblemAccountServices _problemAccountServices;
        private readonly IRescanCustClassificationService _rescanCustClassificationService;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly IRescanNormalClassificationBySportServices _rescanNormalClassificationBySportServices;

        private readonly INewCustomerClassificationServices _services;

        public NewCustomerClassificationTest()
        {
            _newCTSClassificationDataAccess = Substitute.For<INewCustomerClassificationDataAccess>();
            _affectedCustomerClassificationService = Substitute.For<IAffectedDownlineCustomerClassificationService>();
            _problemAccountServices = Substitute.For<IProblemAccountServices>();
            _rescanCustClassificationService = Substitute.For<IRescanCustClassificationService>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _systemParameterServices = Substitute.For<ISystemParameterServices>();
            _rescanNormalClassificationBySportServices = Substitute.For<IRescanNormalClassificationBySportServices>();

            _services = new NewCustomerClassificationServices(
                                _newCTSClassificationDataAccess,
                                _affectedCustomerClassificationService,
                                _problemAccountServices,
                                _rescanCustClassificationService,
                                _custClassToMainDBService,
                                _systemParameterServices,
                                _rescanNormalClassificationBySportServices, null);
        }

        #region GetCategoryForNewCTSCustomer
        [Fact]
        public void GetCategoryForNewCTSCustomer_WhenSuccess()
        {
            // Arrange
            var data = new NewCustomerClassificationModel();

            // Act
            _newCTSClassificationDataAccess.GetNewCTSCustomerCategory().Returns(data);

            var result = _services.GetCategoryForNewCTSCustomer();

            // Assert
            _newCTSClassificationDataAccess.Received(1).GetNewCTSCustomerCategory();
            Assert.NotNull(result);
        }

        [Fact]
        public void GetCategoryForNewCTSCustomer_ThrowException()
        {
            // Arrange

            // Act
            _newCTSClassificationDataAccess.GetNewCTSCustomerCategory().Throws<Exception>();

            Action action = () => _services.GetCategoryForNewCTSCustomer();

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertVVIPForNewCustomers
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InsertVVIPForNewCustomers_WhenSuccess(bool isSuccessResult)
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                VVIPClassification = new List<VVIPCategoryEntity> { new VVIPCategoryEntity() }
            });

            // Act
            _affectedCustomerClassificationService.InsertVVIP(Arg.Any<RescanParametersEntity>(), Arg.Any<VVIPCustomerInsertEntity>()).Returns(isSuccessResult);

            var result = _services.InsertVVIPForNewCustomers(new RescanParametersEntity());

            // Assert
            _affectedCustomerClassificationService.Received(1).InsertVVIP(Arg.Any<RescanParametersEntity>(), Arg.Any<VVIPCustomerInsertEntity>());
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void InsertVVIPForNewCustomers_WhenFailure(bool isSuccess, bool isVVIPClassificationAny)
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, isSuccess);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                VVIPClassification = isVVIPClassificationAny ? new List<VVIPCategoryEntity> { new VVIPCategoryEntity() } : new List<VVIPCategoryEntity>()
            });

            // Act

            _services.InsertVVIPForNewCustomers(new RescanParametersEntity());

            // Assert
            _affectedCustomerClassificationService.Received(0).InsertVVIP(Arg.Any<RescanParametersEntity>(), Arg.Any<VVIPCustomerInsertEntity>());
        }

        [Fact]
        public void InsertVVIPForNewCustomers_ThrowException()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                VVIPClassification = new List<VVIPCategoryEntity> { new VVIPCategoryEntity() }
            });

            // Act
            _affectedCustomerClassificationService.InsertVVIP(Arg.Any<RescanParametersEntity>(), Arg.Any<VVIPCustomerInsertEntity>()).Throws<Exception>();

            Action action = () => _services.InsertVVIPForNewCustomers(new RescanParametersEntity());

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertPAForNewCustomers
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InsertPAForNewCustomers_WhenSuccess(bool isSuccessResult)
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                PAClassification = new List<InsertProblemAccountEntity> { new InsertProblemAccountEntity() }
            });

            // Act
            _problemAccountServices
                .InsertProblemAccountPackage(
                    Arg.Any<IEnumerable<InsertProblemAccountEntity>>(),
                    FunctionId.InsertNewCustJob,
                    CCInputFlow.InsertPACategory,
                    Arg.Any<bool>(),
                    Arg.Any<string>())
                .Returns(new List<long>() { 1, 2, 3 });

            _custClassToMainDBService.PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>())
                .Returns(isSuccessResult);

            var result = _services.InsertPAForNewCustomers(new RescanParametersEntity());

            // Assert
            _problemAccountServices.Received(1)
                .InsertProblemAccountPackage(
                    Arg.Any<IEnumerable<InsertProblemAccountEntity>>(),
                    FunctionId.InsertNewCustJob,
                    CCInputFlow.InsertPACategory,
                    Arg.Any<bool>(),
                    Arg.Any<string>());

            _custClassToMainDBService.Received(1)
                .PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>());


            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void InsertPAForNewCustomers_WhenFailure(bool isSuccess, bool isPAClassificationAny)
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, isSuccess);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                PAClassification = isPAClassificationAny ? new List<InsertProblemAccountEntity> { new InsertProblemAccountEntity() } : new List<InsertProblemAccountEntity>()
            });

            // Act

            _services.InsertPAForNewCustomers(new RescanParametersEntity());

            // Assert
            _problemAccountServices.Received(0)
               .InsertProblemAccountPackage(
                   Arg.Any<IEnumerable<InsertProblemAccountEntity>>(),
                   FunctionId.InsertNewCustJob,
                   CCInputFlow.InsertPACategory,
                   Arg.Any<bool>(),
                   Arg.Any<string>());

            _custClassToMainDBService.Received(0)
                .PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>());
        }

        [Fact]
        public void InsertPAForNewCustomers_ThrowException_InsertProblemAccountPackage()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                PAClassification = new List<InsertProblemAccountEntity> { new InsertProblemAccountEntity() }
            });

            // Act
            _problemAccountServices
                .InsertProblemAccountPackage(
                    Arg.Any<IEnumerable<InsertProblemAccountEntity>>(),
                    FunctionId.InsertNewCustJob,
                    CCInputFlow.InsertPACategory,
                    Arg.Any<bool>(),
                    Arg.Any<string>())
                .Throws<Exception>();

            Action action = () => _services.InsertPAForNewCustomers(new RescanParametersEntity());

            // Assert
            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void InsertPAForNewCustomers_ThrowException_PushExternalProblemAcountPackage()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                PAClassification = new List<InsertProblemAccountEntity> { new InsertProblemAccountEntity() }
            });

            // Act
            _problemAccountServices
                .InsertProblemAccountPackage(
                    Arg.Any<IEnumerable<InsertProblemAccountEntity>>(),
                    Arg.Any<FunctionId>(),
                    Arg.Any<CCInputFlow>(),
                    Arg.Any<bool>(),
                    Arg.Any<string>())
                .Returns(new List<long> { 1, 2, 3 });

            _custClassToMainDBService.PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>())
                .Throws<Exception>();

            Action action = () => _services.InsertPAForNewCustomers(new RescanParametersEntity());

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region ResanNormalForNewCustomers
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ResanNormalForNewCustomers_WhenSuccess(bool isSuccessResult)
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                NormalClassification = new List<NormalRescanEntity> { new NormalRescanEntity() { CustID = 1 }, new NormalRescanEntity() { CustID = 2 } }
            });

            // Act
            _rescanCustClassificationService.RescanCustomerClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<RescanParametersEntity>()).Returns(isSuccessResult);

            var result = _services.ResanNormalForNewCustomers(new RescanParametersEntity());

            // Assert
            _rescanCustClassificationService.Received(1).RescanCustomerClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<RescanParametersEntity>());
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void ResanNormalForNewCustomers_WhenFailure(bool isSuccess, bool isNormalClassificationAny)
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, isSuccess);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                NormalClassification = isNormalClassificationAny ? new List<NormalRescanEntity> { new NormalRescanEntity() } : new List<NormalRescanEntity>()
            });

            // Act

            _services.ResanNormalForNewCustomers(new RescanParametersEntity());

            // Assert
            _rescanCustClassificationService.Received(0).RescanCustomerClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<RescanParametersEntity>());
        }

        [Fact]
        public void ResanNormalForNewCustomers_ThrowException()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                NormalClassification = new List<NormalRescanEntity> { new NormalRescanEntity() }
            });

            // Act
            _rescanCustClassificationService.RescanCustomerClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<RescanParametersEntity>()).Throws<Exception>();

            Action action = () => _services.ResanNormalForNewCustomers(new RescanParametersEntity());

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region ResanNormalBySportForNewCustomers
        [Fact]
        public void ResanNormalBySportForNewCustomers_WhenSuccess()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                NormalClassification = new List<NormalRescanEntity> { new NormalRescanEntity() { CustID = 1 }, new NormalRescanEntity() { CustID = 2 } },
                VVIPClassification = new List<VVIPCategoryEntity> { new VVIPCategoryEntity { CustID = 1 } },
                PAClassification = new List<InsertProblemAccountEntity> { new InsertProblemAccountEntity { CustID = 1 } }
            });

            // Act
            _rescanNormalClassificationBySportServices.PreprocessRescanNormalBySportClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<BySport_FunctionId>());

            _services.ResanNormalBySportForNewCustomers();

            // Assert
            _rescanNormalClassificationBySportServices.Received(1).PreprocessRescanNormalBySportClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<BySport_FunctionId>());
        }

        [Fact]
        public void ResanNormalBySportForNewCustomers_ThrowException()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel()
            {
                NormalClassification = new List<NormalRescanEntity> { new NormalRescanEntity() { CustID = 1 }, new NormalRescanEntity() { CustID = 2 } },
                VVIPClassification = new List<VVIPCategoryEntity> { new VVIPCategoryEntity { CustID = 1 } },
                PAClassification = new List<InsertProblemAccountEntity> { new InsertProblemAccountEntity { CustID = 1 } }
            });

            // Act
            _rescanNormalClassificationBySportServices.When(x => x.PreprocessRescanNormalBySportClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<BySport_FunctionId>())).Throw<Exception>();

            Action action = () => _services.ResanNormalBySportForNewCustomers();

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateComplete
        [Fact]
        public void UpdateComplete_WhenSuccess()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel());

            // Act
            _systemParameterServices.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            _services.UpdateComplete();

            // Assert
            _systemParameterServices.Received(1).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());
        }

        [Fact]
        public void UpdateComplete_WhenFailure()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, false);

            // Act
            _services.UpdateComplete();

            // Assert
            _systemParameterServices.Received(0).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());
        }

        [Fact]
        public void UpdateComplete_ThrowException()
        {
            // Arrange
            var isSuccessField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.IsSuccess);
            isSuccessField.SetValue(_services, true);

            var newCustomerClassificationField = TestExtension.GetPrivateFieldInfo(typeof(NewCustomerClassificationServices), PrivateFieldName.NewCustomerClassification);
            newCustomerClassificationField.SetValue(_services, new NewCustomerClassificationModel());

            // Act
            _systemParameterServices.When(x => x.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>())).Throw<Exception>();

            Action action = () => _services.UpdateComplete();

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion
    }
}
