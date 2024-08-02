using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
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

        #region GetPADailyScanFromQueue
        [Fact]
        public void GetPADailyScanFromQueue_WhenSuccess()
        {
            // Arrange
            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue); ;
            paDailyScanFromQueueField.SetValue(_service, new PADailyScanFromQueueModel());

            // Act
            _dailyClassificationDataAccess.GetPADailyScanFromQueue().Returns(new PADailyScanFromQueueModel());

            var result = _service.GetPADailyScanFromQueue();

            // Assert
            _dailyClassificationDataAccess.Received(1).GetPADailyScanFromQueue();
            Assert.NotNull(result);
        }

        [Fact]
        public void GetPADailyScanFromQueue_ThrowException()
        {
            // Arrange

            // Act
            _dailyClassificationDataAccess.GetPADailyScanFromQueue().Throws<Exception>();

            Action action = () => _service.GetPADailyScanFromQueue();

            // Assert
            _dailyClassificationDataAccess.Received(0).GetPADailyScanFromQueue();
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region GetProblemAccountLosingClassification
        [Fact]
        public void GetProblemAccountLosingClassification_WhenSuccess()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel() { PADailyScanCustIDs = new List<long> { 1, 2, 3 } };
            var functionId = FunctionId.ProblemDailyJob;

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue); ;
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            _problemAccountServices.GetProblemAccountLosingClassification(Arg.Any<IEnumerable<long>>(), functionId).Returns(new List<ProblemAccountLosingClassificationEntity>());

            var result = _service.GetProblemAccountLosingClassification(functionId);

            // Assert
            _problemAccountServices.Received(1).GetProblemAccountLosingClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<FunctionId>());
            Assert.NotNull(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_ReturnEmpty()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel();
            var functionId = FunctionId.ProblemDailyJob;

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue); ;
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            var result = _service.GetProblemAccountLosingClassification(functionId);

            // Assert
            _problemAccountServices.Received(0).GetProblemAccountLosingClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<FunctionId>());
            Assert.Empty(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_ThrowException()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel() { PADailyScanCustIDs = new List<long> { 1, 2, 3 } };
            var functionId = FunctionId.ProblemDailyJob;

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue); ;
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            _problemAccountServices.GetProblemAccountLosingClassification(Arg.Any<IEnumerable<long>>(), functionId).Throws<Exception>();

            Action action = () => _service.GetProblemAccountLosingClassification(functionId);

            // Assert
            _problemAccountServices.Received(0).GetProblemAccountLosingClassification(Arg.Any<IEnumerable<long>>(), Arg.Any<FunctionId>());
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertProblemAccountProbationClassification
        [Fact]
        public void InsertProblemAccountProbationClassification_WhenSuccess()
        {
            // Arrange
            var data = new List<ProblemAccountLosingClassificationEntity> { new ProblemAccountLosingClassificationEntity { } };

            // Act
            _dailyClassificationDataAccess.InsertProblemProbationClassification(Arg.Any<string>()).Returns(new List<InsertPAProbationResult>());

            _service.InsertProblemAccountProbationClassification(0, data);
            // Assert
            _dailyClassificationDataAccess.Received(1).InsertProblemProbationClassification(Arg.Any<string>());
        }

        [Fact]
        public void InsertProblemAccountProbationClassification_ThrowException()
        {
            // Arrange
            var data = new List<ProblemAccountLosingClassificationEntity> { new ProblemAccountLosingClassificationEntity { } };

            // Act
            _dailyClassificationDataAccess.InsertProblemProbationClassification(Arg.Any<string>()).Throws<Exception>();

            Action action = () => _service.InsertProblemAccountProbationClassification(0, data);
            // Assert
            _dailyClassificationDataAccess.Received(0).InsertProblemProbationClassification(Arg.Any<string>());
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertPALogic
        [Fact]
        public void InsertPALogic_WhenSuccess()
        {
            // Arrange
            var data = new List<InsertPAProbationResult> { new InsertPAProbationResult { CustID = 1, IsRobot = true }, new InsertPAProbationResult { CustID = 2, CTSCustID = 2 } };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts); ;
            problemAccountsField.SetValue(_service, data);

            // Act
            _problemAccountServices.InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>());
            _problemAccountServices.InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>());

            _service.InsertPALogic();
            // Assert
            _problemAccountServices.Received(1).InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>());
            _problemAccountServices.Received(1).InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>());
        }

        [Fact]
        public void InsertPALogic_ThrowException()
        {
            // Arrange
            var data = new List<InsertPAProbationResult> { new InsertPAProbationResult { CustID = 1, IsRobot = true }, new InsertPAProbationResult { CustID = 2, CTSCustID = 2 } };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts);
            problemAccountsField.SetValue(_service, data);

            // Act
            _problemAccountServices.When(x => x.InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>())).Throw<Exception>();
            _problemAccountServices.When(x => x.InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>())).Throw<Exception>();

            Action action = () => _service.InsertPALogic();
            // Assert
            _problemAccountServices.Received(0).InsertPALogics(Arg.Any<IEnumerable<CustInfoEntity>>(), Arg.Any<string>());
            _problemAccountServices.Received(0).InsertRobotLogics(Arg.Any<IEnumerable<CustInfoEntity>>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region PushCCToExternal
        [Fact]
        public void PushCCToExternal_WhenSuccess()
        {
            // Arrange
            var data = new List<InsertPAProbationResult> { new InsertPAProbationResult { CustID = 1, IsRobot = true }, new InsertPAProbationResult { CustID = 2, CTSCustID = 2 } };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts); ;
            problemAccountsField.SetValue(_service, data);

            var pushDataToExternal = new PushDataToExternalEntity();

            // Act
            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal).Returns(true);

            var result = _service.PushCCToExternal(pushDataToExternal);

            // Assert
            _custClassToMainDBService.Received(1).PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal);

            Assert.True(result);
        }

        [Fact]
        public void PushCCToExternal_ThrowException()
        {
            // Arrange
            var data = new List<InsertPAProbationResult> { new InsertPAProbationResult { CustID = 1, IsRobot = true }, new InsertPAProbationResult { CustID = 2, CTSCustID = 2 } };

            var problemAccountsField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.ProblemAccounts); ;
            problemAccountsField.SetValue(_service, data);

            var pushDataToExternal = new PushDataToExternalEntity();

            // Act
            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal).Throws<Exception>();

            Action action = () => _service.PushCCToExternal(pushDataToExternal);

            // Assert
            _custClassToMainDBService.Received(0).PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushDataToExternal);

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateComplete
        [Fact]
        public void UpdateComplete_WhenSuccess()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel() { PADailyScanCustIDs = new List<long> { 1, 2, 3 } };

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue); ;
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            _dailyClassificationDataAccess.CompletePADailyScan(Arg.Any<string>());

            var result = _service.UpdateComplete();

            // Assert
            _dailyClassificationDataAccess.Received(1).CompletePADailyScan(Arg.Any<string>());

            Assert.True(result);
        }

        [Fact]
        public void UpdateComplete_ThrowException()
        {
            // Arrange
            var data = new PADailyScanFromQueueModel() { PADailyScanCustIDs = new List<long> { 1, 2, 3 } };

            var paDailyScanFromQueueField = TestExtension.GetPrivateFieldInfo(typeof(DailyProblemClassificationServices), PrivateFieldName.PADailyScanFromQueue); ;
            paDailyScanFromQueueField.SetValue(_service, data);

            // Act
            _dailyClassificationDataAccess.When(x => x.CompletePADailyScan(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.UpdateComplete();

            // Assert
            _dailyClassificationDataAccess.Received(0).CompletePADailyScan(Arg.Any<string>());

            Assert.Throws<Exception>(action);
        }
        #endregion
    }
}
