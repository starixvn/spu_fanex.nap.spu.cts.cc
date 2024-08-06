using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel;
using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sentry;
using System;
using System.Collections.Generic;
using Xunit;
using static Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts.SabaDangerLevelConsts;
using static Fanex.NAP.SPU.CTS.Shares.Constants;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.PushDataToExternal
{
    public class SabaDangerLevelTest
    {
        private readonly ISabaDangerLevelDataAccess _sabaDangerLevelDataAccess;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly ISentryClient _sentryClient;

        private readonly ISabaDangerLevelServices _service;

        public SabaDangerLevelTest()
        {
            _sabaDangerLevelDataAccess = Substitute.For<ISabaDangerLevelDataAccess>();
            _systemParameterServices = Substitute.For<ISystemParameterServices>();
            _sentryClient = Substitute.For<ISentryClient>();

            _service = new SabaDangerLevelServices(_sabaDangerLevelDataAccess, _systemParameterServices);
        }

        #region Main Action

        #region GetSabaDangerLevelForScanning
        [Fact]
        public void GetSabaDangerLevelForScanning_WhenSuccess()
        {
            // Arrange
            var data = new SabaDangerLevelResultEntity
            {
                CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity()
                    }
            };

            // Act
            _sabaDangerLevelDataAccess.GetSabaDangerLevelForScanning().Returns(data);

            var result = _service.GetSabaDangerLevelForScanning();

            // Assert
            _sabaDangerLevelDataAccess.Received(1).GetSabaDangerLevelForScanning();

            Assert.True(result);
        }

        [Fact]
        public void GetSabaDangerLevelForScanning_ThrowException()
        {
            // Arrange

            // Act
            _sabaDangerLevelDataAccess.When(x => x.GetSabaDangerLevelForScanning()).Throw<Exception>();

            Action action = () => _service.GetSabaDangerLevelForScanning();

            // Assert
            _sabaDangerLevelDataAccess.Received(0).GetSabaDangerLevelForScanning();

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateSabaDangerLevelToMainDB
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UpdateSabaDangerLevelToMainDB_WhenSuccess(bool isSuccess)
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    }
                });

            // Act
            _sabaDangerLevelDataAccess.UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>()).Returns(new SabaDangerLevelUpdateResultEntity { IsSuccess = isSuccess });

            _service.UpdateSabaDangerLevelToMainDB(_sentryClient, "TEST", 1);

            // Assert
            _sabaDangerLevelDataAccess.Received(isSuccess ? 2 : 1).UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>());
        }

        [Fact]
        public void UpdateSabaDangerLevelToMainDB_ThrowException()
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    }
                });

            // Act
            _sabaDangerLevelDataAccess.UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>()).Throws<Exception>();

            Action action = () => _service.UpdateSabaDangerLevelToMainDB(_sentryClient, "TEST", 1);

            // Assert
            _sabaDangerLevelDataAccess.Received(0).UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateLogSabaDangerLevel
        [Fact]
        public void UpdateLogSabaDangerLevel_WhenSuccess()
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    },
                    UpdatedResult = new SabaDangerLevelUpdateResultEntity
                    {
                        IsSuccess = true
                    }
                });

            // Act
            _sabaDangerLevelDataAccess.UpdateLogSabaDangerLevel(Arg.Any<string>());

            _service.UpdateLogSabaDangerLevel();

            // Assert
            _sabaDangerLevelDataAccess.Received(1).UpdateLogSabaDangerLevel(Arg.Any<string>());
        }

        [Fact]
        public void UpdateLogSabaDangerLevel_WhenFailure()
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    },
                    UpdatedResult = new SabaDangerLevelUpdateResultEntity
                    {
                        IsSuccess = false
                    }
                });

            // Act
            _service.UpdateLogSabaDangerLevel();

            // Assert
            _sabaDangerLevelDataAccess.Received(0).UpdateLogSabaDangerLevel(Arg.Any<string>());
        }

        [Fact]
        public void UpdateLogSabaDangerLevel_ThrowException()
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    },
                    UpdatedResult = new SabaDangerLevelUpdateResultEntity
                    {
                        IsSuccess = true
                    }
                });

            // Act
            _sabaDangerLevelDataAccess.When(x => x.UpdateLogSabaDangerLevel(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.UpdateLogSabaDangerLevel();

            // Assert
            _sabaDangerLevelDataAccess.Received(0).UpdateLogSabaDangerLevel(Arg.Any<string>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateLastScan
        [Fact]
        public void UpdateLastScan_WhenSuccess()
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    UpdatedResult = new SabaDangerLevelUpdateResultEntity
                    {
                        IsSuccess = true
                    },
                    LastHistoryID = 999
                });

            // Act
            _systemParameterServices.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            _service.UpdateLastScan();

            // Assert
            _systemParameterServices.Received(1).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());
        }

        [Fact]
        public void UpdateLastScan_WhenFailure()
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    UpdatedResult = new SabaDangerLevelUpdateResultEntity
                    {
                        IsSuccess = false
                    },
                    LastHistoryID = 999
                });

            // Act
            _service.UpdateLastScan();

            // Assert
            _systemParameterServices.Received(0).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());
        }

        [Fact]
        public void UpdateLastScan_ThrowException()
        {
            // Arrange
            var sabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.SabaDangerLevel);
            sabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelResultEntity
                {
                    UpdatedResult = new SabaDangerLevelUpdateResultEntity
                    {
                        IsSuccess = true
                    },
                    LastHistoryID = 999
                });

            // Act
            _systemParameterServices.When(x => x.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>())).Throw<Exception>();

            Action action = () => _service.UpdateLastScan();

            // Assert
            _systemParameterServices.Received(0).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #endregion

        #region For Initialzing Data

        #region GetSabaDangerLevelForInitialization
        [Fact]
        public void GetSabaDangerLevelForInitialization_WhenSuccess()
        {
            // Arrange

            // Act
            _sabaDangerLevelDataAccess.GetSabaDangerLevelForInitialization().Returns(new SabaDangerLevelInitResultEntity());

            _service.GetSabaDangerLevelForInitialization();

            // Assert
            _sabaDangerLevelDataAccess.Received(1).GetSabaDangerLevelForInitialization();
        }

        [Fact]
        public void GetSabaDangerLevelForInitialization_ThrowException()
        {
            // Arrange

            // Act
            _sabaDangerLevelDataAccess.GetSabaDangerLevelForInitialization().Throws<Exception>();

            Action action = () => _service.GetSabaDangerLevelForInitialization();

            // Assert
            _sabaDangerLevelDataAccess.Received(0).GetSabaDangerLevelForInitialization();

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateSabaDangerLevelToMainDBForInitialization_WhenSuccess()
        [Theory]
        [InlineData(true)]
        //[InlineData(false)] need check the function LogSentryError in this method 
        public void UpdateSabaDangerLevelToMainDBForInitialization_WhenSuccess(bool isSuccess)
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelInitResultEntity
                {
                    CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    }
                });

            // Act
            _sabaDangerLevelDataAccess.UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>()).Returns(new SabaDangerLevelUpdateResultEntity { IsSuccess = isSuccess });

            _service.UpdateSabaDangerLevelToMainDBForInitialization(_sentryClient, "TEST", 1);

            // Assert
            _sabaDangerLevelDataAccess.Received(isSuccess ? 2 : 1).UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>());

        }

        [Fact]
        public void UpdateSabaDangerLevelToMainDBForInitialization_ThrowException()
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
                new SabaDangerLevelInitResultEntity
                {
                    CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    }
                });

            // Act
            _sabaDangerLevelDataAccess.UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>()).Throws<Exception>();

            Action action = () => _service.UpdateSabaDangerLevelToMainDBForInitialization(_sentryClient, "TEST", 1);

            // Assert
            _sabaDangerLevelDataAccess.Received(0).UpdateDabaDangerLevelToMainDB(Arg.Any<IEnumerable<SabaDangerLevelEntity>>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateLogSabaDangerLevel
        [Fact]
        public void UpdateLogSabaDangerLevelForInitialization_WhenSuccess()
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
               new SabaDangerLevelInitResultEntity
               {
                   CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    },
                   UpdatedResult = new SabaDangerLevelUpdateResultEntity
                   {
                       IsSuccess = true
                   }
               });

            // Act
            _sabaDangerLevelDataAccess.UpdateLogSabaDangerLevel(Arg.Any<string>());

            _service.UpdateLogSabaDangerLevelForInitialization();

            // Assert
            _sabaDangerLevelDataAccess.Received(1).UpdateLogSabaDangerLevel(Arg.Any<string>());
        }

        [Fact]
        public void UpdateLogSabaDangerLevelForInitialization_WhenFailure()
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
               new SabaDangerLevelInitResultEntity
               {
                   CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    },
                   UpdatedResult = new SabaDangerLevelUpdateResultEntity
                   {
                       IsSuccess = false
                   }
               });

            // Act
            _service.UpdateLogSabaDangerLevelForInitialization();

            // Assert
            _sabaDangerLevelDataAccess.Received(0).UpdateLogSabaDangerLevel(Arg.Any<string>());
        }

        [Fact]
        public void UpdateLogSabaDangerLevelForInitialization_ThrowException()
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
               new SabaDangerLevelInitResultEntity
               {
                   CustSabaDangerLevel = new List<SabaDangerLevelEntity>()
                    {
                        new SabaDangerLevelEntity(),
                        new SabaDangerLevelEntity(),
                    },
                   UpdatedResult = new SabaDangerLevelUpdateResultEntity
                   {
                       IsSuccess = true
                   }
               });

            // Act
            _sabaDangerLevelDataAccess.When(x => x.UpdateLogSabaDangerLevel(Arg.Any<string>())).Throw<Exception>();

            Action action = () => _service.UpdateLogSabaDangerLevelForInitialization();

            // Assert
            Assert.Throws<Exception>(action);
        }
        #endregion

        #region UpdateLastScanForInitialization
        [Fact]
        public void UpdateLastScanForInitialization_WhenSuccess()
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
               new SabaDangerLevelInitResultEntity
               {
                   UpdatedResult = new SabaDangerLevelUpdateResultEntity
                   {
                       IsSuccess = true
                   }
               });

            // Act
            _systemParameterServices.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            var result = _service.UpdateLastScanForInitialization();

            // Assert
            _systemParameterServices.Received(2).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            Assert.True(result);
        }

        [Fact]
        public void UpdateLastScanForInitialization_WhenFailure()
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
               new SabaDangerLevelInitResultEntity
               {
                   UpdatedResult = new SabaDangerLevelUpdateResultEntity
                   {
                       IsSuccess = false
                   }
               });

            // Act
            var result = _service.UpdateLastScanForInitialization();

            // Assert

            Assert.False(result);
        }

        [Fact]
        public void UpdateLastScanForInitialization_ThrowException()
        {
            // Arrange
            var initSabaDangerLevelField = TestExtension.GetPrivateFieldInfo(typeof(SabaDangerLevelServices), PrivateFieldName.InitSabaDangerLevel);
            initSabaDangerLevelField.SetValue(_service,
               new SabaDangerLevelInitResultEntity
               {
                   UpdatedResult = new SabaDangerLevelUpdateResultEntity
                   {
                       IsSuccess = true
                   }
               });

            // Act
            _systemParameterServices.When(x => x.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>())).Throw<Exception>();

            Action action = () => _service.UpdateLastScanForInitialization();

            // Assert
            _systemParameterServices.Received(0).UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());

            Assert.Throws<Exception>(action);
        }
        #endregion

        #endregion
    }
}
