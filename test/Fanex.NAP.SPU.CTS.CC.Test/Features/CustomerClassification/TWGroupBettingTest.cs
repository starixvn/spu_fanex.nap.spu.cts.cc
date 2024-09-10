namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using FluentAssertions;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class TWGroupBettingTest
    {
        private readonly ITWGroupBettingDataAccess twGroupBettingDataAccess;
        private readonly ITWGroupBettingService twGroupbettingServices;

        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        public TWGroupBettingTest()
        {
            twGroupBettingDataAccess = Substitute.For<ITWGroupBettingDataAccess>();
            systemParameterDataAccess = Substitute.For<ISystemParameterDataAccess>();
            twGroupbettingServices = new TWGroupBettingService(twGroupBettingDataAccess, systemParameterDataAccess);
        }

        [Fact]
        public void GetLastScanTime_WhenSuccess_RunSuccessfully()
        {
            var output = new SystemParameterEntity { ParameterDataType = "BIGINT", ParameterValue = "1" };

            systemParameterDataAccess.GetPreviousTimingExecution(Main_SystemParameters.TWGroupBetting_LastTransSec).Returns(output);

            var result = twGroupbettingServices.GetConfig_Int64(Main_SystemParameters.TWGroupBetting_LastTransSec);
            result.Should().NotBe(0);
        }

        [Fact]
        public void GetLastTimeCountTotalTicket_WhenSuccess_RunSuccessfully()
        {
            var output = new SystemParameterEntity { ParameterDataType = "INT", ParameterValue = "1" };

            systemParameterDataAccess.GetPreviousTimingExecution(Main_SystemParameters.TWGroupBetting_SkipTransCount).Returns(output);

            var result = twGroupbettingServices.GetConfig_Int(Main_SystemParameters.TWGroupBetting_SkipTransCount);
            result.Should().NotBe(0);
        }

        [Fact]
        public void GetAllRule_WhenSuccess_RunSuccessfully()
        {
            var listRule = new List<TWGroupBettingRule>
            {
                new TWGroupBettingRule()
                {
                    RuleId = 1,
                    RuleName = "RuleType1",
                    LimitSecond = 5
                }
            };
            twGroupBettingDataAccess.GetAllRules().Returns(listRule);

            var result = twGroupbettingServices.GetAllRules();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GetConfigWithoutDefault_WhenSuccess()
        {
            // Arrange
            var data = new SystemParameterEntity
            {
                ParameterValue = "1",
            };

            var paramID = Main_SystemParameters.TWGroupBetting_LastTransSec;

            // Act
            systemParameterDataAccess.GetPreviousTimingExecution(paramID).Returns(data);

            var result = twGroupbettingServices.GetConfigWithoutDefault(paramID);

            // Assert
            systemParameterDataAccess.Received(1).GetPreviousTimingExecution(paramID);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetConfigWithoutDefault_ThrowException()
        {
            // Arrange
            var data = new SystemParameterEntity
            {
                ParameterValue = "1",
            };

            var paramID = Main_SystemParameters.TWGroupBetting_LastTransSec;

            // Act
            systemParameterDataAccess.GetPreviousTimingExecution(paramID).Throws<Exception>();

            Action action = () => twGroupbettingServices.GetConfigWithoutDefault(paramID);

            // Assert
            systemParameterDataAccess.Received(0).GetPreviousTimingExecution(paramID);

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void UpdateConfig_Int64_WhenSuccess()
        {
            // Arrange
            var paramID = Main_SystemParameters.TWGroupBetting_LastTransSec;

            // Act
            systemParameterDataAccess.UpdateTimingExecution(Arg.Any<string>(), paramID);

            twGroupbettingServices.UpdateConfig_Int64(long.MaxValue, paramID);

            // Assert
            systemParameterDataAccess.Received(1).UpdateTimingExecution(Arg.Any<string>(), paramID);
        }

        [Fact]
        public void UpdateConfig_Int64_ThrowException()
        {
            // Arrange
            var paramID = Main_SystemParameters.TWGroupBetting_LastTransSec;

            // Act
            systemParameterDataAccess.When(x => x.UpdateTimingExecution(Arg.Any<string>(), paramID)).Throw<Exception>();

            Action action = () => twGroupbettingServices.UpdateConfig_Int64(long.MaxValue, paramID);

            // Assert
            systemParameterDataAccess.Received(0).UpdateTimingExecution(Arg.Any<string>(), paramID);

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void UpdateConfig_Int_WhenSuccess()
        {
            // Arrange
            var paramID = Main_SystemParameters.TWGroupBetting_LastTransSec;

            // Act
            systemParameterDataAccess.UpdateTimingExecution(Arg.Any<string>(), paramID);

            twGroupbettingServices.UpdateConfig_Int(int.MaxValue, paramID);

            // Assert
            systemParameterDataAccess.Received(1).UpdateTimingExecution(Arg.Any<string>(), paramID);
        }

        [Fact]
        public void UpdateConfig_Int_ThrowException()
        {
            // Arrange
            var paramID = Main_SystemParameters.TWGroupBetting_LastTransSec;

            // Act
            systemParameterDataAccess.When(x => x.UpdateTimingExecution(Arg.Any<string>(), paramID)).Throw<Exception>();

            Action action = () => twGroupbettingServices.UpdateConfig_Int(int.MaxValue, paramID);

            // Assert
            systemParameterDataAccess.Received(0).UpdateTimingExecution(Arg.Any<string>(), paramID);

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void ScanTicket_WhenSuccess()
        {
            // Arrange
            var data = new TWGroupBetting_ScanTicketCriteria();

            // Act
            twGroupBettingDataAccess.ScanTicket(data).Returns(new TWGroupBetting_ScanTicketCriteria());

            var result = twGroupbettingServices.ScanTicket(data);

            // Assert
            twGroupBettingDataAccess.Received(1).ScanTicket(data);

            Assert.NotNull(result);
        }

        [Fact]
        public void ScanTicket_ThowException()
        {
            // Arrange
            var data = new TWGroupBetting_ScanTicketCriteria();

            // Act
            twGroupBettingDataAccess.ScanTicket(data).Throws<Exception>();

            Action action = () => twGroupbettingServices.ScanTicket(data);

            // Assert
            twGroupBettingDataAccess.Received(0).ScanTicket(data);

            Assert.Throws<Exception>(action);
        }

        [Fact]
        public void UpdateTotalTWGBTicket_WhenSuccess()
        {
            // Arrange

            // Act
            twGroupBettingDataAccess.UpdateTotalTWGBTicket(Arg.Any<int>(), Arg.Any<long>()).Returns(999);

            var result = twGroupbettingServices.UpdateTotalTWGBTicket(int.MaxValue, long.MaxValue);

            // Assert
            twGroupBettingDataAccess.Received(1).UpdateTotalTWGBTicket(Arg.Any<int>(), Arg.Any<long>());

            Assert.Equal(999, result);
        }

        [Fact]
        public void UpdateTotalTWGBTicket_ThrowException()
        {
            // Arrange

            // Act
            twGroupBettingDataAccess.UpdateTotalTWGBTicket(Arg.Any<int>(), Arg.Any<long>()).Throws<Exception>();

            Action action = () => twGroupbettingServices.UpdateTotalTWGBTicket(int.MaxValue, long.MaxValue);

            // Assert
            twGroupBettingDataAccess.Received(0).UpdateTotalTWGBTicket(Arg.Any<int>(), Arg.Any<long>());

            Assert.Throws<Exception>(action);
        }
    }
}