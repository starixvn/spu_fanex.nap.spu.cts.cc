using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.JobService;
using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sentry;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.NormalAccountBySportJobServiceConsts;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    public class NormalAccountBySportJobServiceTest
    {
        private readonly ISentryClient _sentryClient;
        private readonly IAppSettings _appSettings;
        private readonly INormalAccountInsertBySportService _normalAccountInsertService;
        private readonly ICustClassToMainDBService _custClassToMainDBService;

        private readonly INormalAccountInsertBySportJobService _service;

        public NormalAccountBySportJobServiceTest()
        {
            _sentryClient = Substitute.For<ISentryClient>();
            _appSettings = Substitute.For<IAppSettings>();
            _normalAccountInsertService = Substitute.For<INormalAccountInsertBySportService>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            _service = new NormalAccountInsertBySportJobService(_sentryClient, _normalAccountInsertService, _custClassToMainDBService);
        }

        #region InsertNormalAccount
        [Fact]
        public void InsertNormalAccount_WhenSuccess()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };

            var insertNormalAccountMethod = TestExtension.GetPrivateMethod(typeof(ResultInsertClassificationBySportModel), PrivateMethodName.InsertNormalAccount);

            // Act
            _normalAccountInsertService.InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>()).Returns(new ResultInsertClassificationBySportModel());
            var result = insertNormalAccountMethod.Invoke(_service, new object[] { data });

            // Assert
            _normalAccountInsertService.Received(1).InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void InsertNormalAccount_ThrowException()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };

            var insertNormalAccountMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.InsertNormalAccount);

            // Act
            _normalAccountInsertService.InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>()).Throws<Exception>();
            Action action = () => insertNormalAccountMethod.Invoke(_service, new object[] { data });

            // Assert
            _normalAccountInsertService.Received(0).InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>());

            Assert.Throws<TargetInvocationException>(action);
        }
        #endregion

        #region PushSpecialCC
        [Fact]
        public void PushSpecialCC_WhenSuccess()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };
            var batchSize = 1000;

            var pushSpecialCCMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.PushSpecialCC);

            // Act
            _custClassToMainDBService.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize).Returns(true);
            var result = pushSpecialCCMethod.Invoke(_service, new object[] { data, batchSize });

            // Assert
            _custClassToMainDBService.Received(1).PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize);

            Assert.NotNull(result);
        }

        [Fact]
        public void PushSpecialCC_ThrowException()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };
            var batchSize = 1000;

            var pushSpecialCCMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.PushSpecialCC);

            // Act
            _custClassToMainDBService.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize).Throws<Exception>();
            Action action = () => pushSpecialCCMethod.Invoke(_service, new object[] { data, batchSize });

            // Assert
            _custClassToMainDBService.Received(0).PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize);

            Assert.Throws<TargetInvocationException>(action);
        }
        #endregion

        #region InsertNormalAndPushCC
        [Fact]
        public void InsertNormalAndPushCC_WhenSuccess()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };
            var batchSize = 1000;

            var insertNormalAndPushCCMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.InsertNormalAndPushCC);

            // Act
            _normalAccountInsertService.InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>()).Returns(new ResultInsertClassificationBySportModel());
            _custClassToMainDBService.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize).Returns(true);
            var result = insertNormalAndPushCCMethod.Invoke(_service, new object[] { data, batchSize });

            // Assert
            _normalAccountInsertService.Received(1).InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>());
            _custClassToMainDBService.Received(1).PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize);

            Assert.True((bool)result);
        }

        [Fact]
        public void InsertNormalAndPushCC_ThrowException()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };
            var batchSize = 1000;

            var insertNormalAndPushCCMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.InsertNormalAndPushCC);

            // Act
            _normalAccountInsertService.InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>()).Returns(new ResultInsertClassificationBySportModel());
            _custClassToMainDBService.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize).Throws<Exception>();
            Action action = () => insertNormalAndPushCCMethod.Invoke(_service, new object[] { data, batchSize });

            // Assert
            Assert.Throws<TargetInvocationException>(action);
        }
        #endregion
    }
}
