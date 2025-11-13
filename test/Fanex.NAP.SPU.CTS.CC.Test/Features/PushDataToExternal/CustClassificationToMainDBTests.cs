namespace Fanex.NAP.SPU.CTS.Tests.Features.PushDataToExternal
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
    using Fanex.NAP.SPU.CTS.Shares;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.CustClassificationToMainDBConsts;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class CustClassificationToMainDBTests
    {
        private readonly ICustClassToMainDBDataAccess _dataAccess;
        private readonly IAntiFraudCenterServices _antiFraudCenterServices;

        private readonly ICustClassToMainDBService _service;

        public CustClassificationToMainDBTests()
        {
            _antiFraudCenterServices = Substitute.For<IAntiFraudCenterServices>();

            _dataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            _service = new CustClassToMainDBService(_dataAccess, _antiFraudCenterServices);
            LogExtension.ConfigByPassForUnitTest();
        }

        [Fact]
        public void PushExternalPackageByCustId_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custIDs = new List<long>() { 1, 2, 3 };
            var parameter = new PushDataToExternalEntity
            {
                ExternalBatchSize = 50,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = false,
                ServiceName = "Test"
            };

            // Act
            var exception = Record.Exception(() => _service.PushExternalPackageByCustId(custIDs, parameter));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void PushExternalProblemAcountPackage_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custIDs = new List<long>() { 1, 2, 3 };
            var parameter = new PushDataToExternalEntity
            {
                ExternalBatchSize = 50,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = false,
                ServiceName = "Test"
            };

            // Act           
            var exception = Record.Exception(() => _service.PushExternalProblemAcountPackage(custIDs, parameter));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void PushCustomerClassification_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var customerClass = new List<LatestClassificationEntity>()
            {
                new LatestClassificationEntity { CustId = 1, Category = 20200, CustomerClass = 10 }
            };

            // Act   
            _dataAccess.UpdateCustomerClassification(customerClass, RoleGroupType.Member).Returns(new Shares.Models.DBResult());

            var exception = Record.Exception(() => _service.PushCustomerClassification(customerClass, RoleGroupType.Member, 1, "Test"));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void PushCCBySportToMainDB_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custInfos = new List<InsertNormalAccountCustInfoEntity>()
            {
                new InsertNormalAccountCustInfoEntity (new CustInfoClassificationBySportEntity { CustID = 1, SportGroup = 1 })
            };

            // Act

            var exception = Record.Exception(() => _service.PushCCBySportToMainDB(custInfos, "Test", 50));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void RetryPushCCBySportToMainDB_WhenSuccess_DoesNotThrowException()
        {
            // Arrange
            var custInfos = new List<GetCustomerClassificationBySportEntity>()
            {
                new GetCustomerClassificationBySportEntity { CustID = 1, SportID = 1, CustomerClass = 10 }
            };

            // Act

            var exception = Record.Exception(() => _service.RetryPushCCBySportToMainDB(custInfos, 2, "Test"));

            // Assert
            Assert.Null(exception);
        }

        #region PushCustClassificationToMainDB
        [Fact]
        public void PushCustClassificationToMainDB_WhenSuccess()
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity()
            };

            var pushCustClassificationToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushCustClassificationToMainDB);


            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member).Returns(new Shares.Models.DBResult());

            var result = pushCustClassificationToMainDBMethod.Invoke(_service, new object[] { data, RoleGroupType.Member, string.Empty });
            // Assert
            _dataAccess.Received(1).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member);

            Assert.True((bool)result);
        }

        [Fact]
        public void PushCustClassificationToMainDB_WhenParamEmpty()
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
            };

            var pushCustClassificationToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushCustClassificationToMainDB);


            // Act
            var result = pushCustClassificationToMainDBMethod.Invoke(_service, new object[] { data, RoleGroupType.Member, string.Empty });
            // Assert
            _dataAccess.Received(0).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member);

            Assert.True((bool)result);
        }

        [Fact]
        public void PushCustClassificationToMainDB_ThrowException()
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity()
            };

            var pushCustClassificationToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushCustClassificationToMainDB);


            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member).Throws<Exception>();

            var result = pushCustClassificationToMainDBMethod.Invoke(_service, new object[] { data, RoleGroupType.Member, string.Empty });
            // Assert
            _dataAccess.Received(4).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member);

            Assert.False((bool)result);
        }
        #endregion

        #region GetLatestCCAndDangerLevel
        [Fact]
        public void GetLatestCCAndDangerLevel_WhenSuccess()
        {
            // Arrange
            var data = new List<long>() { 1, 2, 3, 4 };

            var getLatestCCAndDangerLevelMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetLatestCCAndDangerLevel);

            // Act
            _dataAccess.GetLatestCCAndDangerLevel(Arg.Any<IEnumerable<long>>()).Returns(new List<LatestCCDangerLevelEntity>());

            var result = getLatestCCAndDangerLevelMethod.Invoke(_service, new object[] { data });

            // Assert
            _dataAccess.Received(1).GetLatestCCAndDangerLevel(Arg.Any<IEnumerable<long>>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetLatestCCAndDangerLevel_WhenParamEmpty()
        {
            // Arrange
            var getLatestCCAndDangerLevelMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetLatestCCAndDangerLevel);

            // Act
            _dataAccess.GetLatestCCAndDangerLevel(Arg.Any<IEnumerable<long>>()).Returns(new List<LatestCCDangerLevelEntity>());

            var result = getLatestCCAndDangerLevelMethod.Invoke(_service, new object[] { Enumerable.Empty<long>() });

            // Assert
            _dataAccess.Received(0).GetLatestCCAndDangerLevel(Arg.Any<IEnumerable<long>>());

            Assert.Empty((IEnumerable<LatestCCDangerLevelEntity>)result);
        }

        [Fact]
        public void GetLatestCCAndDangerLevel_ThrowException()
        {
            // Arrange
            var data = new List<long>() { 1, 2, 3, 4 };

            var getLatestCCAndDangerLevelMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetLatestCCAndDangerLevel);

            // Act
            _dataAccess.GetLatestCCAndDangerLevel(Arg.Any<IEnumerable<long>>()).Throws<Exception>();

            Action action = () => getLatestCCAndDangerLevelMethod.Invoke(_service, new object[] { data });

            // Assert
            _dataAccess.Received(0).GetLatestCCAndDangerLevel(Arg.Any<IEnumerable<long>>());

            Assert.Throws<TargetInvocationException>(action);
        }
        #endregion

        #region GetCustomerClassificationBySports
        [Fact]
        public void GetCustomerClassificationBySports_WhenSuccess()
        {
            // Arrange
            var data = new List<InsertProblemAccountInfoBySportModel>() { new InsertProblemAccountInfoBySportModel() };

            var getCustomerClassificationBySportsMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetCustomerClassificationBySports);

            // Act
            _dataAccess.GetCustomerClassificationBySports(Arg.Any<IEnumerable<BaseCustomerInfoBySportModel>>()).Returns(new List<GetCustomerClassificationBySportEntity>());

            var result = getCustomerClassificationBySportsMethod.Invoke(_service, new object[] { data });

            // Assert
            _dataAccess.Received(1).GetCustomerClassificationBySports(Arg.Any<IEnumerable<BaseCustomerInfoBySportModel>>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetCustomerClassificationBySports_WhenParamEmpty()
        {
            // Arrange
            var getCustomerClassificationBySportsMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetCustomerClassificationBySports);

            // Act
            _dataAccess.GetCustomerClassificationBySports(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>()).Returns(new List<GetCustomerClassificationBySportEntity>());

            var result = getCustomerClassificationBySportsMethod.Invoke(_service, new object[] { Enumerable.Empty<InsertNormalAccountCustInfoEntity>() });

            // Assert
            _dataAccess.Received(0).GetCustomerClassificationBySports(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>());

            Assert.Empty((IEnumerable<GetCustomerClassificationBySportEntity>)result);
        }

        [Fact]
        public void GetCustomerClassificationBySports_ThrowException()
        {
            // Arrange
            var data = new List<InsertProblemAccountInfoBySportModel>() { new InsertProblemAccountInfoBySportModel() };

            var getCustomerClassificationBySportsMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetCustomerClassificationBySports);

            // Act
            _dataAccess.GetCustomerClassificationBySports(Arg.Any<IEnumerable<BaseCustomerInfoBySportModel>>()).Throws<Exception>();

            Action action = () => getCustomerClassificationBySportsMethod.Invoke(_service, new object[] { data });

            // Assert
            _dataAccess.Received(0).GetCustomerClassificationBySports(Arg.Any<IEnumerable<BaseCustomerInfoBySportModel>>());

            Assert.Throws<TargetInvocationException>(action);
        }
        #endregion

        #region PushDangerLevel
        [Fact]
        public void PushDangerLevel_WhenSuccess()
        {
            // Arrange
            var data = new List<DangerLevelEntity>
            {
                new DangerLevelEntity()
            };

            var retryTime = 3;
            var serviceName = string.Empty;

            var pushDangerLevelMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushDangerLevel);


            // Act
            _dataAccess.UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>()).Returns(new Shares.Models.DBResult());

            var result = pushDangerLevelMethod.Invoke(_service, new object[] { data, 999, retryTime, serviceName });
            // Assert
            _dataAccess.Received(1).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            Assert.True((bool)result);
        }

        [Fact]
        public void PushDangerLevel_WhenParamEmpty()
        {
            // Arrange
            var retryTime = 3;
            var serviceName = string.Empty;

            var pushDangerLevelMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushDangerLevel);


            // Act
            var result = pushDangerLevelMethod.Invoke(_service, new object[] { Enumerable.Empty<DangerLevelEntity>(), 999, retryTime, serviceName });

            // Assert
            _dataAccess.Received(0).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            Assert.True((bool)result);
        }

        [Fact]
        public void PushDangerLevel_ThrowException()
        {
            // Arrange
            var data = new List<DangerLevelEntity>
            {
                new DangerLevelEntity()
            };

            var retryTime = 3;
            var serviceName = string.Empty;

            var pushDangerLevelMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushDangerLevel);


            // Act
            _dataAccess.UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>()).Throws<Exception>();

            var result = pushDangerLevelMethod.Invoke(_service, new object[] { data, 999, retryTime, serviceName });
            // Assert
            _dataAccess.Received(4).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            Assert.False((bool)result);
        }
        #endregion

        #region UpdateDangerLevelToMainDB
        [Fact]
        public void UpdateDangerLevelToMainDB_WhenSuccess()
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity() { CustID = 1, DangerLevel = 1 },   // for ABI
                new LatestCCDangerLevelEntity() { CustID = 2, Danger1 = 2 },       // for Ori
                new LatestCCDangerLevelEntity() { CustID = 3, DangerLevel = 1, Danger1 = 2 } // both
            };

            var updateDangerLevelToMainDBMethod = TestExtension.GetPrivateMethod(
                typeof(CustClassToMainDBService),
                PrivateMethodName.UpdateDangerLevelToMainDB
            );

            _dataAccess
                .UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>())
                .Returns(new Shares.Models.DBResult());

            // Act
            var result = updateDangerLevelToMainDBMethod.Invoke(_service, new object[] { data, "TestService" });

            // Assert
            _dataAccess.Received(1).UpdateDangerLevelToMainDB(
                Arg.Is<IEnumerable<DangerLevelEntity>>(x => x.Any(e => e.DangerLevel == 1)),
                (int)DangerType.ABI
            );

            _dataAccess.Received(1).UpdateDangerLevelToMainDB(
                Arg.Is<IEnumerable<DangerLevelEntity>>(x => x.Any(e => e.DangerLevel == 2)),
                (int)DangerType.Ori
            );

            Assert.True((bool)result);
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_WhenParamEmpty()
        {
            // Arrange
            var updateDangerLevelToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.UpdateDangerLevelToMainDB);

            // Act
            var result = updateDangerLevelToMainDBMethod.Invoke(_service, new object[] { Enumerable.Empty<LatestCCDangerLevelEntity>(), string.Empty });
            // Assert
            _dataAccess.Received(0).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            Assert.True((bool)result);
        }

        [Fact]
        public void UpdateDangerLevelToMainDB_ThrowException()
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity() { DangerLevel = 1, DangerLevelType = 1 },
                new LatestCCDangerLevelEntity() { DangerLevel = 1, DangerLevelType = 1 },
                new LatestCCDangerLevelEntity() { DangerLevel = 2, DangerLevelType = 2 },
            };

            var updateDangerLevelToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.UpdateDangerLevelToMainDB);

            // Act
            _dataAccess.UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>()).Throws<Exception>();

            var result = updateDangerLevelToMainDBMethod.Invoke(_service, new object[] { data, string.Empty });
            // Assert
            _dataAccess.Received(4).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            Assert.False((bool)result);
        }
        #endregion

        #region IsPushExternalSuccess
        [Fact]
        public void IsPushExternalSuccess_WhenSuccess()
        {
            // Arrange
            var lastestClassifications = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity() { DangerLevel = 1, DangerLevelType = 1 },
            };

            var paras = new PushDataToExternalEntity() { IsUpdateDangerToMainDB = true, RoleGroup = RoleGroupType.Member };

            var isPushExternalSuccessMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.IsPushExternalSuccess);

            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member).Returns(new Shares.Models.DBResult());

            _dataAccess.UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>()).Returns(new Shares.Models.DBResult());

            var result = isPushExternalSuccessMethod.Invoke(_service, new object[] { lastestClassifications, paras });
            // Assert
            _dataAccess.Received(1).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            _dataAccess.Received(1).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member);

            Assert.True((bool)result);
        }

        [Fact]
        public void IsPushExternalSuccess_WhenSuccess_NotUpdateDanger()
        {
            // Arrange
            var lastestClassifications = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity() { DangerLevel = 1, DangerLevelType = 1 },
            };

            var paras = new PushDataToExternalEntity() { IsUpdateDangerToMainDB = false, RoleGroup = RoleGroupType.Member };

            var isPushExternalSuccessMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.IsPushExternalSuccess);

            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member).Returns(new Shares.Models.DBResult());

            var result = isPushExternalSuccessMethod.Invoke(_service, new object[] { lastestClassifications, paras });
            // Assert
            _dataAccess.Received(0).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            _dataAccess.Received(1).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member);

            Assert.True((bool)result);
        }

        [Fact]
        public void IsPushExternalSuccess_WhenSuccess_ParamEmpty()
        {
            // Arrange

            var paras = new PushDataToExternalEntity() { IsUpdateDangerToMainDB = false };

            var isPushExternalSuccessMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.IsPushExternalSuccess);

            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member).Returns(new Shares.Models.DBResult());

            var result = isPushExternalSuccessMethod.Invoke(_service, new object[] { Enumerable.Empty<LatestCCDangerLevelEntity>(), paras });
            // Assert
            _dataAccess.Received(0).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            _dataAccess.Received(0).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), RoleGroupType.Member);

            Assert.True((bool)result);
        }
        #endregion
    }
}