using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialGroupBetting;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using FluentAssertions;
using NSubstitute;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RealtimeTWInitialGroupBetting
{
    public class TWInitialGroupBettingTest
    {
        private readonly ITWInitialGroupBettingDataAccess _dataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly IBaseCustomerClassificationDataAccess _baseDataAccess;
        private readonly IProblemAccountDataAccess _problemAccountDataAccess;
        private readonly ITWInitialGroupBettingService _service;

        public TWInitialGroupBettingTest()
        {
            _dataAccess = Substitute.For<ITWInitialGroupBettingDataAccess>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            _problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();

            _service = new TWInitialGroupBettingService(
                _baseDataAccess,
                _problemAccountDataAccess,
                _custClassToMainDBService,
                _dataAccess);
        }

        #region GetChanges
        [Fact]
        public void GetChanges_WhenSuccess()
        {
            // Arrange

            // Act
            _dataAccess.GetChanges().Returns(new TWInitialGBGetChangesModel());

            var result = _service.GetChanges();

            // Assert
            _dataAccess.Received(1).GetChanges();

            Assert.NotNull(result);
        }
        #endregion

        #region UpdateNextScannedTime
        [Fact]
        public void UpdateNextScannedTime_WhenSuccess()
        {
            // Arrange

            // Act
            _dataAccess.UpdateNextScannedTime(Arg.Any<DateTime>(), Arg.Any<long>());

            _service.UpdateNextScannedTime(DateTime.MinValue, 999);

            // Assert
            _dataAccess.Received(1).UpdateNextScannedTime(Arg.Any<DateTime>(), Arg.Any<long>());
        }
        #endregion

        #region InsertTWInitalGBClassification
        [Fact]
        public void InsertTWInitalGBClassification_WhenSuccess()
        {
            // Arrange
            var twInitialSmartCusts = new List<TWInitialGBGetChangesEntity>()
            {
                 new TWInitialGBGetChangesEntity
                 {
                     CustID = 1, DetectedDate = DateTime.Now,
                 }
            };

            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = 500,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = false,
                ServiceName = "Realtime TW Initial Smart JobService"
            };

            // Act
            _dataAccess.GetCustInfos(Arg.Any<string>()).Returns(new List<PotentialPACustInfoEntity> { new PotentialPACustInfoEntity { CTSCustID = 999 } });

            _baseDataAccess.When(x => x.InsertCustomerClassification(CCInputFlow.Potential, Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel()));

            var result = _service.InsertTWInitalGBClassification(twInitialSmartCusts, paras);

            // Assert
            result.Should().BeTrue();
        }
        #endregion
    }
}
