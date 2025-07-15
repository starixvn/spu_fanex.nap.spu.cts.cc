namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
    using FluentAssertions;
    using NSubstitute;
    
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.RobotAIDetectionConsts;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RobotDetectionServiceTest
    {
        private readonly IRobotAIDetectionDataAccess _dataAccess;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly IBaseCustomerClassificationDataAccess _baseDataAccess;
        private readonly IProblemAccountDataAccess _problemAccountDataAccess;
        private readonly IRobotAIDetectionService _service;

        public RobotDetectionServiceTest()
        {
            _dataAccess = Substitute.For<IRobotAIDetectionDataAccess>();
            _systemParameterServices = Substitute.For<ISystemParameterServices>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            _problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            _service = new RobotAIDetectionService(_dataAccess
                , _systemParameterServices
                , _custClassToMainDBService
                , _baseDataAccess
                , _problemAccountDataAccess
                );
        }

        [Fact]
        public void GetRobotDetections_WhenSuccess()
        {
            // Arrange
            var robotDetectionsField = TestExtension.GetPrivateFieldInfo(typeof(RobotAIDetectionService), PrivateFieldName.RobotDetections);

            // Act
            _systemParameterServices.GetLastScannedId(Arg.Any<CTS_SystemParameters>()).Returns(999);

            _dataAccess.GetRobotDetections(Arg.Any<long>()).Returns(new List<RobotAIEntity>());

            _service.GetRobotDetections(CTS_SystemParameters.RobotDetection_LastID);

            var robotDetection = robotDetectionsField.GetValue(_service);

            // Assert
            _systemParameterServices.Received(1).GetLastScannedId(Arg.Any<CTS_SystemParameters>());

            _dataAccess.Received(1).GetRobotDetections(Arg.Any<long>());

            Assert.NotNull(robotDetection);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InsertAIRobotPackage_WhenSuccess(bool isSuccess)
        {
            // Arrange
            var robotDetectionsField = TestExtension.GetPrivateFieldInfo(typeof(RobotAIDetectionService), PrivateFieldName.RobotDetections);
            robotDetectionsField.SetValue(_service, new List<RobotAIEntity>()
            {
                new RobotAIEntity(), new RobotAIEntity(), new RobotAIEntity()
            });

            var pushConfig = new PushDataToExternalEntity();

            // Act
            _baseDataAccess
                .InsertCustomerClassification(Arg.Any<CCInputFlow>(), Arg.Any<InsertCustomerClassificationModel>())
                .Returns(new ResultInsertCustomerClassificationModel()
                {
                    CustInfos = new List<CustInfoClassificationEntity>() { new CustInfoClassificationEntity() }
                });

            _custClassToMainDBService.PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushConfig).Returns(isSuccess);

            var result = _service.InsertAIRobotPackage(999, pushConfig);

            // Assert
            _baseDataAccess.Received(1)
                .InsertCustomerClassification(Arg.Any<CCInputFlow>(), Arg.Any<InsertCustomerClassificationModel>());

            _custClassToMainDBService.Received(1).PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), pushConfig);

            Assert.True(isSuccess == result);
        }

        [Fact]
        public void Process_WhenSuccess_ReturnTrue()
        {
            // Arrange
            var paras = new PushDataToExternalEntity
            {
                ExternalBatchSize = 1,
                ServiceName = "robotDetectionServiceName",
                IsWriteLog = false,
                IsSyncAFCData = false,
            };

            var expectedRobotAIID = 1;
            var expectedRobotAI = new List<RobotAIEntity>()
            {
                new RobotAIEntity { CustID = 1, CategoryID = 1 , RobotDetectionID = 1, LastModifiedDate = DateTime.Now },
                new RobotAIEntity { CustID = 2, CategoryID = 1 , RobotDetectionID = 2, LastModifiedDate = DateTime.Now}
            };

            // Act
            _systemParameterServices.GetLastScannedId(CTS_SystemParameters.RobotDetection_LastID).Returns(expectedRobotAIID);

            _dataAccess.GetRobotDetections(expectedRobotAIID).Returns((expectedRobotAI));

            var result = _service.InsertAIRobotPackage(1, paras);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void UpdateLastScannedId_WhenSuccess()
        {
            // Arrange
            var robotDetectionsField = TestExtension.GetPrivateFieldInfo(typeof(RobotAIDetectionService), PrivateFieldName.RobotDetections);
            robotDetectionsField.SetValue(_service, new List<RobotAIEntity>
            {
                new RobotAIEntity() { RobotDetectionID = 999 }
            });

            // Act
            _systemParameterServices.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            _service.UpdateLastScannedId(CTS_SystemParameters.AssociatedGroupLastCTSCustID);

            // Assert
            _systemParameterServices.Received(1).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());
        }
    }
}
