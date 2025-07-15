using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.JobService;
using Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.NormalAccountBySportJobServiceConsts;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    public class NormalAccountBySportJobServiceTest
    {
        private readonly IAppSettings _appSettings;
        private readonly INormalAccountInsertBySportService _normalAccountInsertService;
        private readonly ICustClassToMainDBService _custClassToMainDBService;

        private readonly INormalAccountInsertBySportJobService _service;

        public NormalAccountBySportJobServiceTest()
        {
            _appSettings = Substitute.For<IAppSettings>();
            _normalAccountInsertService = Substitute.For<INormalAccountInsertBySportService>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();

            _service = new NormalAccountInsertBySportJobService(_normalAccountInsertService, _custClassToMainDBService);
        }

        #region InsertNormalAccount
        [Fact]
        public void InsertNormalAccount_WhenSuccess()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };

            var insertNormalAccountMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.InsertNormaBySport);

            // Act
            _normalAccountInsertService.InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>()).Returns(new ResultInsertClassificationBySportModel()
            {
                CustInfos = new List<CustInfoClassificationBySportEntity>()
            });

            var result = insertNormalAccountMethod.Invoke(_service, new object[] { data, 5000 });

            // Assert
            _normalAccountInsertService.Received(1).InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>());

            Assert.NotNull(result);
        }

        [Fact]
        public void InsertNormalAccount_ThrowException()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };

            var insertNormalAccountMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.InsertNormaBySport);

            // Act
            _normalAccountInsertService.InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>()).Throws<Exception>();
            Action action = () => insertNormalAccountMethod.Invoke(_service, new object[] { data, 5000 });

            // Assert
            _normalAccountInsertService.Received(0).InsertNormalClassificationBySport(Arg.Any<IEnumerable<NormalAccountBySportEntity>>(), Arg.Any<string>());

            Assert.Throws<TargetInvocationException>(action);
        }
        #endregion

        #region PushSpecialCCBySport
        [Fact]
        public void PushSpecialCCBySport_WhenSuccess()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };
            var batchSize = 1000;

            var insertNormalAndPushCCMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.PushSpecialCCBySport);

            // Act
            _custClassToMainDBService.PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize).Returns(true);
            var result = insertNormalAndPushCCMethod.Invoke(_service, new object[] { data, batchSize });

            // Assert
            _custClassToMainDBService.Received(1).PushCCBySportToMainDB(Arg.Any<IEnumerable<InsertNormalAccountCustInfoEntity>>(), Arg.Any<string>(), batchSize);

            Assert.True((bool)result);
        }

        [Fact]
        public void PushSpecialCCBySport_ThrowException()
        {
            // Arrange
            var data = new List<NormalAccountBySportEntity> { new NormalAccountBySportEntity() };
            var batchSize = 1000;

            var insertNormalAndPushCCMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountInsertBySportJobService), PrivateMethodName.PushSpecialCCBySport);

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
