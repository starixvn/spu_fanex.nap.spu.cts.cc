namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{   
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport;
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
            dailyClassificationServices = new DailyNormalClassificationBySportServices(dailyDataAccess, systemParameterService);
        }

        [Fact]
        public void GetCustClassificationForDailyScanning_WhenSuccess_ReturnNotException()
        {
            Action action = () => dailyClassificationServices.Start().ScanningDailyNormalClassification();

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
