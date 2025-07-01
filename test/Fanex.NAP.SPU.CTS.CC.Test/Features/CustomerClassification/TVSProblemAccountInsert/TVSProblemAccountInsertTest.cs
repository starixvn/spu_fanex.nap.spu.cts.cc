using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TVSProblemAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using Xunit;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.TVSProblemAccountInsert
{
    public class TVSProblemAccountInsertTest
    {
        private readonly ITVSProblemAccountDataAccess _tvsProblemAccountDataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly IBaseCustomerClassificationDataAccess _baseDataAccess;
        private readonly IProblemAccountDataAccess _problemAccountDataAccess;

        private readonly ITVSProblemAccountServices _service;

        public TVSProblemAccountInsertTest()
        {
            _tvsProblemAccountDataAccess = Substitute.For<ITVSProblemAccountDataAccess>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            _problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();

            _service = new TVSProblemAccountServices(
                                _tvsProblemAccountDataAccess,
                                _baseDataAccess, 
                                _problemAccountDataAccess, 
                                _custClassToMainDBService);
        }

        #region GetTVSProblemAccounts
        [Fact]
        public void GetTVSProblemAccounts_WhenSuccess()
        {
            // Arrange

            // Act
            _tvsProblemAccountDataAccess.GetTWProblemAccount(Arg.Any<DateTime?>()).Returns(new TVSProblemAccountResultEntity());

            var result = _service.GetTVSProblemAccounts(DateTime.Now);
            // Assert
            _tvsProblemAccountDataAccess.Received(1).GetTWProblemAccount(Arg.Any<DateTime?>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetTVSProblemAccounts_NotNull()
        {
            // Arrange

            // Act
            _tvsProblemAccountDataAccess.GetTWProblemAccount(Arg.Any<DateTime?>()).Returns(default(TVSProblemAccountResultEntity));

            var result = _service.GetTVSProblemAccounts(DateTime.Now);
            // Assert
            _tvsProblemAccountDataAccess.Received(1).GetTWProblemAccount(Arg.Any<DateTime?>());

            Assert.NotNull(result);
        }

        [Fact]
        public void GetTVSProblemAccounts_ThrowException()
        {
            // Arrange

            // Act
            _tvsProblemAccountDataAccess.GetTWProblemAccount(Arg.Any<DateTime?>()).Throws<Exception>();

            Action action = () => _service.GetTVSProblemAccounts(DateTime.Now);
            // Assert
            _tvsProblemAccountDataAccess.Received(0).GetTWProblemAccount(Arg.Any<DateTime?>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region InsertTVSProblemAccounts
        [Fact]
        public void InsertTVSProblemAccounts_WhenSuccess()
        {
            // Arrange
            var data = new List<TVSProblemAccountEntity> { new TVSProblemAccountEntity(), new TVSProblemAccountEntity(), new TVSProblemAccountEntity(), new TVSProblemAccountEntity() };
            var paras = new PushDataToExternalEntity { ExternalBatchSize = 1 };

            // Act
            _baseDataAccess.InsertCustomerClassification(CCInputFlow.InsertPAReason, Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel());
            _custClassToMainDBService.PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>()).Returns(true);

            var result = _service.InsertTVSProblemAccounts(data, paras);
            // Assert
            _custClassToMainDBService.Received(4).PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>());

            Assert.True(result);
        }

        [Fact]
        public void InsertTVSProblemAccounts_ThrowException()
        {
            // Arrange
            var data = new List<TVSProblemAccountEntity> { new TVSProblemAccountEntity(), new TVSProblemAccountEntity(), new TVSProblemAccountEntity(), new TVSProblemAccountEntity() };
            var paras = new PushDataToExternalEntity { ExternalBatchSize = 1 };

            // Act
            _baseDataAccess.InsertCustomerClassification(CCInputFlow.InsertPAReason, Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel());
            _custClassToMainDBService.PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>()).Throws<Exception>();

            Action action = () => _service.InsertTVSProblemAccounts(data, paras);
            // Assert
            _custClassToMainDBService.Received(0).PushExternalProblemAcountPackage(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>());

            Assert.Throws<Exception>(action);
        }
        #endregion
    }
}
