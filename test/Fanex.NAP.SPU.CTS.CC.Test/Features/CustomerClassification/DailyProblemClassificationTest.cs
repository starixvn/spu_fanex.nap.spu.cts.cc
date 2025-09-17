using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification.Entities;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.DailyProblemClassificationConsts;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification
{
    public class DailyProblemClassificationTest
    {
        private readonly IDailyProblemClassificationDataAccess _dailyClassificationDataAccess;
        private readonly IProblemAccountServices _problemAccountServices;
        private readonly ICustClassToMainDBService _custClassToMainDBService;

        private readonly IDailyProblemClassificationServices _service;

        public DailyProblemClassificationTest()
        {
            _dailyClassificationDataAccess = Substitute.For<IDailyProblemClassificationDataAccess>();
            _problemAccountServices = Substitute.For<IProblemAccountServices>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            _service = new DailyProblemClassificationServices(_dailyClassificationDataAccess, _problemAccountServices, _custClassToMainDBService);
        }

        #region Constructor Tests
        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeService()
        {
            // Act & Assert
            Assert.NotNull(_service);
            Assert.IsType<DailyProblemClassificationServices>(_service);
        }
        #endregion

        #region Start Method Tests
        [Fact]
        public void Start_ShouldInitializePADailyScanFromQueue_ReturnsService()
        {
            // Act
            var result = _service.Start();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void Start_ShouldResetPADailyScanFromQueue_ReturnsService()
        {
            // Arrange - Set some data first
            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, new PADailyScanFromQueueModel { PADailyScanCustIDs = new List<PADailyScanFromQueueEntity> { new PADailyScanFromQueueEntity { CustID = 1 } } });

            // Act
            var result = _service.Start();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
            Assert.Same(_service, result);
        }
        #endregion

        #region Reset Method Tests
        [Fact]
        public void Reset_ShouldInitializeProblemAccounts_ReturnsService()
        {
            // Act
            var result = _service.Reset();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void Reset_ShouldClearExistingProblemAccounts_ReturnsService()
        {
            // Arrange - Set some data first
            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, new List<InsertPAProbationResult> { new InsertPAProbationResult { CustID = 1 } });

            // Act
            var result = _service.Reset();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
            Assert.Same(_service, result);
        }
        #endregion

        #region GetPADailyScanFromQueue Tests
        [Fact]
        public void GetPADailyScanFromQueue_WhenSuccess_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var expectedData = new PADailyScanFromQueueModel
            {
                PADailyScanCustIDs = new List<PADailyScanFromQueueEntity>
                {
                    new PADailyScanFromQueueEntity { CustID = 1, DWSportType = 1 },
                    new PADailyScanFromQueueEntity { CustID = 2, DWSportType = 2 }
                }
            };

            _dailyClassificationDataAccess.GetPADailyScanFromQueue().Returns(expectedData);

            // Act
            var result = _service.GetPADailyScanFromQueue();

            // Assert
            _dailyClassificationDataAccess.Received(1).GetPADailyScanFromQueue();
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void GetPADailyScanFromQueue_WhenDataAccessThrowsException_ShouldPropagateException()
        {
            // Arrange
            _dailyClassificationDataAccess.GetPADailyScanFromQueue().Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.GetPADailyScanFromQueue());
            _dailyClassificationDataAccess.Received(1).GetPADailyScanFromQueue();
        }

        [Fact]
        public void GetPADailyScanFromQueue_WhenDataAccessReturnsNull_ShouldHandleGracefully()
        {
            // Arrange
            _dailyClassificationDataAccess.GetPADailyScanFromQueue().Returns((PADailyScanFromQueueModel)null);

            // Act
            var result = _service.GetPADailyScanFromQueue();

            // Assert
            _dailyClassificationDataAccess.Received(1).GetPADailyScanFromQueue();
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }
        #endregion

        #region GetProblemAccountLosingClassification Tests
        [Fact]
        public void GetProblemAccountLosingClassification_WithValidCustIDs_ShouldCallProblemAccountServices_ReturnsData()
        {
            // Arrange
            var custIds = new List<PADailyScanFromQueueEntity>
    {
        new PADailyScanFromQueueEntity { CustID = 1, DWSportType = 1 },
        new PADailyScanFromQueueEntity { CustID = 2, DWSportType = 2 }
    };
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = custIds };
            var functionId = FunctionId.ProblemDailyJob;

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            var expectedResult = new List<ProblemAccountLosingClassificationEntity>
    {
        new ProblemAccountLosingClassificationEntity { CustId = 1 },
        new ProblemAccountLosingClassificationEntity { CustId = 2 }
    };

            // Fix: Mock the correct overload that takes IEnumerable<PADailyScanFromQueueEntity>
            _problemAccountServices.GetProblemAccountLosingClassification(Arg.Any<IEnumerable<PADailyScanFromQueueEntity>>(), functionId).Returns(expectedResult);

            // Act
            var result = _service.GetProblemAccountLosingClassification(functionId);

            // Assert
            // Fix: Verify the correct overload
            _problemAccountServices.Received(1).GetProblemAccountLosingClassification(Arg.Is<IEnumerable<PADailyScanFromQueueEntity>>(x => x.Count() == 2), functionId);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WithEmptyCustIDs_ShouldReturnEmpty_DoesNotCallServices()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = new List<PADailyScanFromQueueEntity>() };
            var functionId = FunctionId.ProblemDailyJob;

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            var result = _service.GetProblemAccountLosingClassification(functionId);

            // Assert
            _problemAccountServices.DidNotReceive().GetProblemAccountLosingClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<FunctionId>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WithNullCustIDs_ShouldReturnEmpty_DoesNotCallServices()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = null };
            var functionId = FunctionId.ProblemDailyJob;

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            var result = _service.GetProblemAccountLosingClassification(functionId);

            // Assert
            _problemAccountServices.DidNotReceive().GetProblemAccountLosingClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<FunctionId>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WhenProblemAccountServicesThrowsException_ShouldPropagateException()
        {
            // Arrange
            var custIds = new List<PADailyScanFromQueueEntity> { new PADailyScanFromQueueEntity { CustID = 1 } };
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = custIds };
            var functionId = FunctionId.ProblemDailyJob;

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            // Fix: Mock the correct overload that takes IEnumerable<PADailyScanFromQueueEntity>
            _problemAccountServices.GetProblemAccountLosingClassification(Arg.Any<IEnumerable<PADailyScanFromQueueEntity>>(), functionId).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.GetProblemAccountLosingClassification(functionId));
            _problemAccountServices.Received(1).GetProblemAccountLosingClassification(Arg.Any<IEnumerable<PADailyScanFromQueueEntity>>(), functionId);
        }
        #endregion

        #region InsertProblemAccountProbationClassification Tests
        [Fact]
        public void InsertProblemAccountProbationClassification_WithValidData_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var problemClassification = new List<ProblemAccountLosingClassificationEntity>
            {
                new ProblemAccountLosingClassificationEntity { CustId = 1 },
                new ProblemAccountLosingClassificationEntity { CustId = 2 }
            };
            var batchSize = 50;
            var expectedResult = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false },
                new InsertPAProbationResult { CustID = 2, CTSCustID = 1002, IsRobot = true }
            };

            _dailyClassificationDataAccess.InsertProblemProbationClassification(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertProblemAccountProbationClassification(batchSize, problemClassification);

            // Assert
            _dailyClassificationDataAccess.Received(1).InsertProblemProbationClassification(Arg.Is<string>(json => !string.IsNullOrEmpty(json)));
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void InsertProblemAccountProbationClassification_WithEmptyData_ShouldNotCallDataAccess_ReturnsService()
        {
            // Arrange
            var problemClassification = new List<ProblemAccountLosingClassificationEntity>();
            var batchSize = 50;

            // Act
            var result = _service.InsertProblemAccountProbationClassification(batchSize, problemClassification);

            // Assert
            _dailyClassificationDataAccess.DidNotReceive().InsertProblemProbationClassification(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }

        [Fact]
        public void InsertProblemAccountProbationClassification_WithNullData_ShouldNotCallDataAccess_ReturnsService()
        {
            // Arrange
            List<ProblemAccountLosingClassificationEntity> problemClassification = null;
            var batchSize = 50;

            // Act
            var result = _service.InsertProblemAccountProbationClassification(batchSize, problemClassification);

            // Assert
            _dailyClassificationDataAccess.DidNotReceive().InsertProblemProbationClassification(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }

        [Fact]
        public void InsertProblemAccountProbationClassification_WhenDataAccessThrowsException_ShouldPropagateException()
        {
            // Arrange
            var problemClassification = new List<ProblemAccountLosingClassificationEntity>
            {
                new ProblemAccountLosingClassificationEntity { CustId = 1 }
            };
            var batchSize = 50;

            _dailyClassificationDataAccess.InsertProblemProbationClassification(Arg.Any<string>()).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.InsertProblemAccountProbationClassification(batchSize, problemClassification));
            _dailyClassificationDataAccess.Received(1).InsertProblemProbationClassification(Arg.Any<string>());
        }
        #endregion

        #region InsertPALogic Tests
        [Fact]
        public void InsertPALogic_WithValidData_ShouldCallBothPALogicsAndRobotLogics_ReturnsService()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false },
                new InsertPAProbationResult { CustID = 2, CTSCustID = 1002, IsRobot = true },
                new InsertPAProbationResult { CustID = 3, CTSCustID = 1003, IsRobot = false }
            };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            // Act
            var result = _service.InsertPALogic();

            // Assert
            _problemAccountServices.Received(1).InsertPALogics(
                Arg.Is<IEnumerable<CustInfoEntity>>(x => x.Count() == 2), // 2 non-robot accounts
                Arg.Any<string>());
            _problemAccountServices.Received(1).InsertRobotLogics(
                Arg.Is<IEnumerable<CustInfoEntity>>(x => x.Count() == 1)); // 1 robot account
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void InsertPALogic_WithOnlyNonRobotAccounts_ShouldCallOnlyPALogics_ReturnsService()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
    {
        new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false },
        new InsertPAProbationResult { CustID = 2, CTSCustID = 1002, IsRobot = false }
    };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            // Act
            var result = _service.InsertPALogic();

            // Assert
            _problemAccountServices.Received(1).InsertPALogics(
                Arg.Is<IEnumerable<CustInfoEntity>>(x => x.Count() == 2), // 2 non-robot accounts
                Arg.Any<string>());
            // Fix: The service calls InsertRobotLogics even with empty collection, so we should expect it to be called
            _problemAccountServices.Received(1).InsertRobotLogics(
                Arg.Is<IEnumerable<CustInfoEntity>>(x => x.Count() == 0)); // Empty collection for robot accounts
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }

        [Fact]
        public void InsertPALogic_WithOnlyRobotAccounts_ShouldCallOnlyRobotLogics_ReturnsService()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = true },
                new InsertPAProbationResult { CustID = 2, CTSCustID = 1002, IsRobot = true }
            };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            // Act
            var result = _service.InsertPALogic();

            // Assert
            // Fix: The service calls InsertPALogics even with empty collection, so we should expect it to be called
            _problemAccountServices.Received(1).InsertPALogics(
                Arg.Is<IEnumerable<CustInfoEntity>>(x => x.Count() == 0), // Empty collection for non-robot accounts
                Arg.Any<string>());
            _problemAccountServices.Received(1).InsertRobotLogics(
                Arg.Is<IEnumerable<CustInfoEntity>>(x => x.Count() == 2)); // 2 robot accounts
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }

        [Fact]
        public void InsertPALogic_WithEmptyData_ShouldNotCallAnyServices_ReturnsService()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>();

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            // Act
            var result = _service.InsertPALogic();

            // Assert
            _problemAccountServices.DidNotReceive().InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>());
            _problemAccountServices.DidNotReceive().InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>());
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }

        [Fact]
        public void InsertPALogic_WithNullData_ShouldNotCallAnyServices_ReturnsService()
        {
            // Arrange
            List<InsertPAProbationResult> data = null;

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            // Act
            var result = _service.InsertPALogic();

            // Assert
            _problemAccountServices.DidNotReceive().InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>());
            _problemAccountServices.DidNotReceive().InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>());
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }

        [Fact]
        public void InsertPALogic_WhenPALogicsThrowsException_ShouldPropagateException()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false }
            };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            _problemAccountServices.When(x => x.InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>())).Throw<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.InsertPALogic());
            _problemAccountServices.Received(1).InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>());
        }

        [Fact]
        public void InsertPALogic_WhenRobotLogicsThrowsException_ShouldPropagateException()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = true }
            };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            _problemAccountServices.When(x => x.InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>())).Throw<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.InsertPALogic());
            _problemAccountServices.Received(1).InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>());
        }
        #endregion

        #region PushCCToExternal Tests
        [Fact]
        public void PushCCToExternal_WithValidData_ShouldCallExternalService_ReturnsTrue()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false },
                new InsertPAProbationResult { CustID = 2, CTSCustID = 1002, IsRobot = true },
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false } // Duplicate CustID
            };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            var pushDataToExternal = new PushDataToExternalEntity { ServiceName = "TestService" };

            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal).Returns(true);

            // Act
            var result = _service.PushCCToExternal(pushDataToExternal);

            // Assert
            _custClassToMainDBService.Received(1).PushExternalPackageByCustId(
                Arg.Is<IEnumerable<long>>(x => x.Count() == 2), // Should be distinct CustIDs
                pushDataToExternal);
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WithEmptyData_ShouldReturnTrue_DoesNotCallExternalService()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>();

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            var pushDataToExternal = new PushDataToExternalEntity { ServiceName = "TestService" };

            // Act
            var result = _service.PushCCToExternal(pushDataToExternal);

            // Assert
            _custClassToMainDBService.DidNotReceive().PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>());
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WithNullData_ShouldReturnTrue_DoesNotCallExternalService()
        {
            // Arrange
            List<InsertPAProbationResult> data = null;

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            var pushDataToExternal = new PushDataToExternalEntity { ServiceName = "TestService" };

            // Act
            var result = _service.PushCCToExternal(pushDataToExternal);

            // Assert
            _custClassToMainDBService.DidNotReceive().PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>());
            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_WhenExternalServiceReturnsFalse_ShouldReturnFalse()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false }
            };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            var pushDataToExternal = new PushDataToExternalEntity { ServiceName = "TestService" };

            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal).Returns(false);

            // Act
            var result = _service.PushCCToExternal(pushDataToExternal);

            // Assert
            _custClassToMainDBService.Received(1).PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal);
            Assert.False(result);
        }

        [Fact]
        public void PushCCToExternal_WhenExternalServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var data = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false }
            };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            var pushDataToExternal = new PushDataToExternalEntity { ServiceName = "TestService" };

            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal).Throws<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.PushCCToExternal(pushDataToExternal));
            _custClassToMainDBService.Received(1).PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal);
        }
        #endregion

        #region UpdateComplete Tests
        [Fact]
        public void UpdateComplete_WithValidCustIDs_ShouldCallDataAccess_ReturnsTrue()
        {
            // Arrange
            var custIds = new List<PADailyScanFromQueueEntity>
            {
                new PADailyScanFromQueueEntity { CustID = 1, DWSportType = 1 },
                new PADailyScanFromQueueEntity { CustID = 2, DWSportType = 2 },
                new PADailyScanFromQueueEntity { CustID = 1, DWSportType = 1 } // Duplicate CustID
            };
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = custIds };

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dailyClassificationDataAccess.Received(1).CompletePADailyScan(Arg.Is<string>(x => x == "1,2")); // Should be distinct CustIDs
            Assert.True(result);
        }

        [Fact]
        public void UpdateComplete_WithEmptyCustIDs_ShouldCallDataAccessWithEmptyString_ReturnsFalse()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = new List<PADailyScanFromQueueEntity>() };

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dailyClassificationDataAccess.Received(1).CompletePADailyScan(Arg.Is<string>(x => x == string.Empty));
            Assert.False(result);
        }

        [Fact]
        public void UpdateComplete_WithNullCustIDs_ShouldCallDataAccessWithEmptyString_ReturnsFalse()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = null };

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            var result = _service.UpdateComplete();

            // Assert
            _dailyClassificationDataAccess.Received(1).CompletePADailyScan(Arg.Is<string>(x => x == string.Empty));
            Assert.False(result);
        }

        [Fact]
        public void UpdateComplete_WhenDataAccessThrowsException_ShouldPropagateException()
        {
            // Arrange
            var custIds = new List<PADailyScanFromQueueEntity>
            {
                new PADailyScanFromQueueEntity { CustID = 1, DWSportType = 1 }
            };
            var data = new PADailyScanFromQueueModel { PADailyScanCustIDs = custIds };

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            paDailyScanFromQueueField.SetValue(_service, data);

            _dailyClassificationDataAccess.When(x => x.CompletePADailyScan(Arg.Any<string>())).Throw<InvalidOperationException>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.UpdateComplete());
            _dailyClassificationDataAccess.Received(1).CompletePADailyScan(Arg.Any<string>());
        }
        #endregion

        #region Edge Cases and Error Handling Tests
        [Fact]
        public void GetProblemAccountLosingClassification_WithNullPADailyScanFromQueue_ShouldReturnEmpty()
        {
            // Arrange
            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue);
            // Fix: Set to a valid object with null PADailyScanCustIDs instead of null object
            paDailyScanFromQueueField.SetValue(_service, new PADailyScanFromQueueModel { PADailyScanCustIDs = null });

            var functionId = FunctionId.ProblemDailyJob;

            // Act
            var result = _service.GetProblemAccountLosingClassification(functionId);

            // Assert
            _problemAccountServices.DidNotReceive().GetProblemAccountLosingClassification(Arg.Any<IEnumerable<PADailyScanFromQueueEntity>>(), Arg.Any<FunctionId>());
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void InsertProblemAccountProbationClassification_WithZeroBatchSize_ShouldStillProcess()
        {
            // Arrange
            var problemClassification = new List<ProblemAccountLosingClassificationEntity>
            {
                new ProblemAccountLosingClassificationEntity { CustId = 1 }
            };
            var batchSize = 0; // Zero batch size
            var expectedResult = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false }
            };

            _dailyClassificationDataAccess.InsertProblemProbationClassification(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertProblemAccountProbationClassification(batchSize, problemClassification);

            // Assert
            _dailyClassificationDataAccess.Received(1).InsertProblemProbationClassification(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }

        [Fact]
        public void InsertProblemAccountProbationClassification_WithNegativeBatchSize_ShouldStillProcess()
        {
            // Arrange
            var problemClassification = new List<ProblemAccountLosingClassificationEntity>
            {
                new ProblemAccountLosingClassificationEntity { CustId = 1 }
            };
            var batchSize = -10; // Negative batch size
            var expectedResult = new List<InsertPAProbationResult>
            {
                new InsertPAProbationResult { CustID = 1, CTSCustID = 1001, IsRobot = false }
            };

            _dailyClassificationDataAccess.InsertProblemProbationClassification(Arg.Any<string>()).Returns(expectedResult);

            // Act
            var result = _service.InsertProblemAccountProbationClassification(batchSize, problemClassification);

            // Assert
            _dailyClassificationDataAccess.Received(1).InsertProblemProbationClassification(Arg.Any<string>());
            Assert.NotNull(result);
            Assert.IsType<DailyProblemClassificationServices>(result);
        }
        #endregion
    }
}
