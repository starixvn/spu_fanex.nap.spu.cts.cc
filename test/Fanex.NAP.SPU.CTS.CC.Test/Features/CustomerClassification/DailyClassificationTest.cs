namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class DailyClassificationTest
    {
        private readonly IDailyNormalClassificationDataAccess dailyClassificationDataAccess;
        private readonly IDailyNormalClassificationServices dailyClassificationServices;
        private readonly ISystemParameterServices systemParameterService;

        public DailyClassificationTest()
        {
            dailyClassificationDataAccess = Substitute.For<IDailyNormalClassificationDataAccess>();
            dailyClassificationServices = new DailyNormalClassificationServices(dailyClassificationDataAccess, systemParameterService);
        }

        [Fact]
        public void GetCustClassificationForDailyScanning_WhenSuccess_ReturnNotException()
        {
            Action action = () => dailyClassificationServices.Start().ScanningDailyNormalClassification(FunctionId.NormalDailyJob, true);

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void ClassifyCustomerCategoryByCustIds_WhenSuccess_ReturnNotException()
        {
            var custIds = new List<long> { 1, 2 };

            dailyClassificationDataAccess.ScanningDailyNormalClassification(string.Join(",", custIds), FunctionId.NormalDailyJob, true);

            Action action = () => dailyClassificationServices.ScanningDailyNormalClassification(custIds, FunctionId.NormalDailyJob, true);
            action.Should().NotThrow<Exception>();
        }
    }
}