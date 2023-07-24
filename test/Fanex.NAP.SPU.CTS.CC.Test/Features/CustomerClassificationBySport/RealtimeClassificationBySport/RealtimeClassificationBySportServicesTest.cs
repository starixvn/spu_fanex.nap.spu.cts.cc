namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class RealtimeClassificationBySportServicesTest
    {
        private readonly IRealtimeClassificationBySportDataAccess realtimeDataAccess;
        private readonly IRealtimeClassificationBySportServices realtimeService;

        public RealtimeClassificationBySportServicesTest()
        {
            realtimeDataAccess = Substitute.For<IRealtimeClassificationBySportDataAccess>();
            realtimeService = new RealtimeClassificationBySportServices(realtimeDataAccess, null);
        }

        [Fact]
        public void GetNewPlaceBetCustomers_WhenSuccess_NotException()
        {
            // Arrange

            // Act
            realtimeService.GetNewPlaceBetCustomers();

            // Assert
            realtimeDataAccess.Received(1).GetNewPlaceBetCustomers();
        }

        [Fact]
        public void PreprocessCustomerClassRealtime_WhenSuccess_NotException()
        {
            // Arrange           
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>()
                {
                    new CustClassificationRealtimeBySportEntity { CustId = 1, SportGroup = 1 },
                    new CustClassificationRealtimeBySportEntity { CustId = 1, SportGroup = 2 }
                },
                ScannedMaxId = 7
            };

            // Act
            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);
            realtimeService.Start().GetNewPlaceBetCustomers().PreprocessCustomerClassRealtime(100);

            // Assert
            realtimeDataAccess.Received(0).PreprocessCustomerClassRealtime(new List<CustClassificationRealtimeBySportEntity>());
        }

        [Fact]
        public void CompleteCustomerRealtime_WhenSuccess_NotException()
        {
            // Arrange
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>() {
                    new CustClassificationRealtimeBySportEntity {  CustId = 1, SportGroup = 1},
                    new CustClassificationRealtimeBySportEntity{  CustId = 1, SportGroup = 2}
                },
                ScannedMaxId = 7
            };

            // Act  
            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);

            realtimeService.Start()
                .GetNewPlaceBetCustomers()
                .CompleteCustomerRealtime();

            // Assert
            realtimeDataAccess.Received(1).CompleteCustomerRealtime(modelCustomers.ScannedMaxId);
        }
        
    }
}
