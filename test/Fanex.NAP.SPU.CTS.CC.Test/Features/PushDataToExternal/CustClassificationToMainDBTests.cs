namespace Fanex.NAP.SPU.CTS.Tests.Features.PushDataToExternal
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Sentry;
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
        private readonly ISentryClient _sentryClient;

        private readonly ICustClassToMainDBService _service;

        public CustClassificationToMainDBTests()
        {
            _sentryClient = Substitute.For<ISentryClient>();

            _antiFraudCenterServices = Substitute.For<IAntiFraudCenterServices>();

            _dataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            _service = new CustClassToMainDBService(_dataAccess, _sentryClient, _antiFraudCenterServices);
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
            _dataAccess.UpdateCustomerClassification(customerClass).Returns(new Shares.Models.DBResult());

            var exception = Record.Exception(() => _service.PushCustomerClassification(customerClass, 1, "Test"));

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
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PushCustClassificationToMainDB_WhenSuccess(bool isSMACustomer)
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity()
            };

            var pushCustClassificationToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushCustClassificationToMainDB);


            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), isSMACustomer).Returns(new Shares.Models.DBResult());

            var result = pushCustClassificationToMainDBMethod.Invoke(_service, new object[] { data, string.Empty, isSMACustomer });
            // Assert
            _dataAccess.Received(1).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), isSMACustomer);

            Assert.True((bool)result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PushCustClassificationToMainDB_WhenParamEmpty(bool isSMACustomer)
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
            };

            var pushCustClassificationToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushCustClassificationToMainDB);


            // Act
            var result = pushCustClassificationToMainDBMethod.Invoke(_service, new object[] { data, string.Empty, isSMACustomer });
            // Assert
            _dataAccess.Received(0).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), isSMACustomer);

            Assert.True((bool)result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PushCustClassificationToMainDB_ThrowException(bool isSMACustomer)
        {
            // Arrange
            var data = new List<LatestCCDangerLevelEntity>
            {
                new LatestCCDangerLevelEntity()
            };

            var pushCustClassificationToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.PushCustClassificationToMainDB);


            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>()).Throws<Exception>();

            var result = pushCustClassificationToMainDBMethod.Invoke(_service, new object[] { data, string.Empty, isSMACustomer });
            // Assert
            _dataAccess.Received(4).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>(), isSMACustomer);

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
            var data = new List<InsertNormalAccountCustInfoEntity>() { new InsertNormalAccountCustInfoEntity(new NormalAccountBySportEntity()) };

            var getCustomerClassificationBySportsMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetCustomerClassificationBySports);

            // Act
            _dataAccess.GetCustomerClassificationBySports(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>()).Returns(new List<GetCustomerClassificationBySportEntity>());

            var result = getCustomerClassificationBySportsMethod.Invoke(_service, new object[] { data });

            // Assert
            _dataAccess.Received(1).GetCustomerClassificationBySports(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>());

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
            var data = new List<InsertNormalAccountCustInfoEntity>() { new InsertNormalAccountCustInfoEntity(new NormalAccountBySportEntity()) };

            var getCustomerClassificationBySportsMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.GetCustomerClassificationBySports);

            // Act
            _dataAccess.GetCustomerClassificationBySports(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>()).Throws<Exception>();

            Action action = () => getCustomerClassificationBySportsMethod.Invoke(_service, new object[] { data });

            // Assert
            _dataAccess.Received(0).GetCustomerClassificationBySports(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>());

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
                new LatestCCDangerLevelEntity() { DangerLevel = 1, DangerLevelType = 1 },
                new LatestCCDangerLevelEntity() { DangerLevel = 1, DangerLevelType = 1 },
                new LatestCCDangerLevelEntity() { DangerLevel = 2, DangerLevelType = 2 },
            };

            var updateDangerLevelToMainDBMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.UpdateDangerLevelToMainDB);

            // Act
            _dataAccess.UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>()).Returns(new Shares.Models.DBResult());

            var result = updateDangerLevelToMainDBMethod.Invoke(_service, new object[] { data, string.Empty });
            // Assert
            _dataAccess.Received(2).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

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

            var paras = new PushDataToExternalEntity() { IsUpdateDangerToMainDB = true };

            var isPushExternalSuccessMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.IsPushExternalSuccess);

            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>()).Returns(new Shares.Models.DBResult());

            _dataAccess.UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>()).Returns(new Shares.Models.DBResult());

            var result = isPushExternalSuccessMethod.Invoke(_service, new object[] { lastestClassifications, paras });
            // Assert
            _dataAccess.Received(1).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            _dataAccess.Received(1).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>());

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

            var paras = new PushDataToExternalEntity() { IsUpdateDangerToMainDB = false };

            var isPushExternalSuccessMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.IsPushExternalSuccess);

            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>()).Returns(new Shares.Models.DBResult());

            var result = isPushExternalSuccessMethod.Invoke(_service, new object[] { lastestClassifications, paras });
            // Assert
            _dataAccess.Received(0).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            _dataAccess.Received(1).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>());

            Assert.True((bool)result);
        }

        [Fact]
        public void IsPushExternalSuccess_WhenSuccess_ParamEmpty()
        {
            // Arrange

            var paras = new PushDataToExternalEntity() { IsUpdateDangerToMainDB = false };

            var isPushExternalSuccessMethod = TestExtension.GetPrivateMethod(typeof(CustClassToMainDBService), PrivateMethodName.IsPushExternalSuccess);

            // Act
            _dataAccess.UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>()).Returns(new Shares.Models.DBResult());

            var result = isPushExternalSuccessMethod.Invoke(_service, new object[] { Enumerable.Empty<LatestCCDangerLevelEntity>(), paras });
            // Assert
            _dataAccess.Received(0).UpdateDangerLevelToMainDB(Arg.Any<IEnumerable<DangerLevelEntity>>(), Arg.Any<int>());

            _dataAccess.Received(0).UpdateCustomerClassification(Arg.Any<IEnumerable<LatestClassificationEntity>>());

            Assert.True((bool)result);
        }
        #endregion
    }
}