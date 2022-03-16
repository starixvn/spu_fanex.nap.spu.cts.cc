namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.TWGroupBetting;
    using FluentAssertions;
    using Newtonsoft.Json;
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
            var result = twGroupbettingServices.GetLastScanTime(SystemParameters.TWGroupBetting_LastScanTime);
            result.Should().NotBe(DateTime.MinValue);
        }

        [Fact]
        public void GetLastTimeCountTotalTicket_WhenSuccess_RunSuccessfully()
        {
            var result = twGroupbettingServices.GetLastTimeCountTotalTicket(SystemParameters.TWGroupBetting_LastTimeCountTotalTicket);
            result.Should().NotBe(DateTime.MinValue);
        }
        
        [Fact]
        public void GetAllRule_WhenSuccess_RunSuccessfully()
        {
            var result = twGroupbettingServices.GetAllRules();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void CountTotalTicket_WhenSuccess_RunSuccessfully()
        {
            var lastUpdatedTotalTicketTime = DateTime.Now;
            var result = twGroupbettingServices.CountTotalTicket(lastUpdatedTotalTicketTime);
            result.Should().BeAfter(DateTime.MinValue);
        }

        //public void Abc()
        //{
        //    DateTime fromDate = DateTime.MinValue;
        //    DateTime toDate = DateTime.MinValue;
        //    int step = 5; 
        //    twGroupbettingServices.ScanTicket(fromDate, toDate, step);
        //}

        //[Theory]
        //[InlineData(0)]
        //[InlineData(-1)]
        //public void GetProbationCustForScanning_WhenFail_ReturnEmptyList(int batchSize)
        //{
        //    probationClassificationDataAccess.GetProbationCustForScanning(batchSize).Returns(new List<ProbationEntity>());

        //    probationClassificationServices.Start().GetProbationCustForScanning(batchSize);
        //    var result = probationClassificationServices.CustomerClassification;

        //    result.Should().BeEquivalentTo(new List<ProbationEntity>());
        //}
    }
}