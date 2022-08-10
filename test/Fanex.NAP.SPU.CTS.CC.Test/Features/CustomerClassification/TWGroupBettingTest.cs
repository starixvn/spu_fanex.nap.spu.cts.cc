namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting.Entities;
    using FluentAssertions;
    using NSubstitute;
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
            var result = twGroupbettingServices.GetConfig_Int64(SystemParameters.TWGroupBetting_LastTransID);
            result.Should().NotBe(0);
        }

        [Fact]
        public void GetLastTimeCountTotalTicket_WhenSuccess_RunSuccessfully()
        {
            var result = twGroupbettingServices.GetConfig_Int(SystemParameters.TWGroupBetting_SkipTransCount);
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
    }
}