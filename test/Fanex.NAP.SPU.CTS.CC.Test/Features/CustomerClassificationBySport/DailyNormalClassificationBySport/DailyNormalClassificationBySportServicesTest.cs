namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{   
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using Xunit;

    public class DailyNormalClassificationBySportServicesTest
    {
        private readonly IDailyNormalClassificationBySportDataAccess dailyDataAccess;
        private readonly IDailyNormalClassificationBySportServices dailyClassificationServices;
        private readonly ISystemParameterServices systemParameterService;

        public DailyNormalClassificationBySportServicesTest()
        {
            dailyDataAccess = Substitute.For<IDailyNormalClassificationBySportDataAccess>();
            dailyClassificationServices = new DailyNormalClassificationBySportServices(dailyDataAccess, systemParameterService, null);
        }

        [Fact]
        public void GetCustClassificationForDailyScanning_WhenSuccess_ReturnNotException()
        {
            Action action = () => dailyClassificationServices.Start().GetCustClassificationForDailyScanning();

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void PreprocessDailyNormalClassification_WhenSuccess_ReturnNotException()
        {
            Action action = () => dailyClassificationServices.PreprocessDailyNormalClassification();

            action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void UpdateLastScan_WhenSuccess_ReturnNotException()
        {            
            Action action = () => dailyClassificationServices.UpdateLastScan();
            action.Should().NotThrow<Exception>();
        }
    }
}
