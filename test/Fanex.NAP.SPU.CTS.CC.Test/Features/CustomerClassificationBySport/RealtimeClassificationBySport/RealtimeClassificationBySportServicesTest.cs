namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.RealtimeClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RealtimeClassificationBySport;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class RealtimeClassificationBySportServicesTest
    {
        private readonly IRealtimeClassificationBySportDataAccess realtimeDataAccess;
        private readonly IRealtimeClassificationBySportServices realtimeService;

        public RealtimeClassificationBySportServicesTest()
        {
            realtimeDataAccess = Substitute.For<IRealtimeClassificationBySportDataAccess>();
            realtimeService = new RealtimeClassificationBySportServices(realtimeDataAccess);
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
        public void PreprocessCustomerClassRealtime_WhenHasCustomersAndCustomerNormals_ShouldCallPreprocessCustomerClassRealtime()
        {
            // Arrange
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>()
                {
                    new CustClassificationRealtimeBySportEntity { CustId = 1, SportGroup = 1 },
                    new CustClassificationRealtimeBySportEntity { CustId = 2, SportGroup = 2 },
                    new CustClassificationRealtimeBySportEntity { CustId = 3, SportGroup = 1 }
                },
                ScannedMaxId = 7
            };

            var modelCategory = new GetCustomerCategoryBySportModel
            {
                CustomerNormals = new List<long> { 1, 2 },
                CustomerPADailyScans = new List<long>()
            };

            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);
            realtimeDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(modelCategory);

            // Act
            realtimeService.Start()
                .GetNewPlaceBetCustomers()
                .GetCustomerCategory(100)
                .PreprocessCustomerClassRealtime(100);

            // Assert
            realtimeDataAccess.Received(1).PreprocessCustomerClassRealtime(
                Arg.Is<IEnumerable<CustClassificationRealtimeBySportEntity>>(x => 
                    x.Count() == 2 && 
                    x.Any(c => c.CustId == 1) && 
                    x.Any(c => c.CustId == 2)));
        }

        [Fact]
        public void PreprocessCustomerClassRealtime_WhenNoCustomers_ShouldNotCallPreprocessCustomerClassRealtime()
        {
            // Arrange
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>(),
                ScannedMaxId = 0
            };

            var modelCategory = new GetCustomerCategoryBySportModel
            {
                CustomerNormals = new List<long> { 1, 2 },
                CustomerPADailyScans = new List<long>()
            };

            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);

            // Act
            realtimeService.Start()
                .GetNewPlaceBetCustomers()
                .PreprocessCustomerClassRealtime(100);

            // Assert
            realtimeDataAccess.DidNotReceive().PreprocessCustomerClassRealtime(Arg.Any<IEnumerable<CustClassificationRealtimeBySportEntity>>());
        }

        [Fact]
        public void PreprocessCustomerClassRealtime_WhenNoCustomerNormals_ShouldNotCallPreprocessCustomerClassRealtime()
        {
            // Arrange
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>()
                {
                    new CustClassificationRealtimeBySportEntity { CustId = 1, SportGroup = 1 },
                    new CustClassificationRealtimeBySportEntity { CustId = 2, SportGroup = 2 }
                },
                ScannedMaxId = 7
            };

            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);

            // Act
            realtimeService.Start()
                .GetNewPlaceBetCustomers()
                .PreprocessCustomerClassRealtime(100);

            // Assert
            realtimeDataAccess.DidNotReceive().PreprocessCustomerClassRealtime(Arg.Any<IEnumerable<CustClassificationRealtimeBySportEntity>>());
        }

        [Fact]
        public void PreprocessCustomerClassRealtime_WhenNoMatchingCustomers_ShouldNotCallPreprocessCustomerClassRealtime()
        {
            // Arrange
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>()
                {
                    new CustClassificationRealtimeBySportEntity { CustId = 1, SportGroup = 1 },
                    new CustClassificationRealtimeBySportEntity { CustId = 2, SportGroup = 2 }
                },
                ScannedMaxId = 7
            };

            var modelCategory = new GetCustomerCategoryBySportModel
            {
                CustomerNormals = new List<long> { 3, 4 },
                CustomerPADailyScans = new List<long>()
            };

            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);
            realtimeDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(modelCategory);

            // Act
            realtimeService.Start()
                .GetNewPlaceBetCustomers()
                .GetCustomerCategory(100)
                .PreprocessCustomerClassRealtime(100);

            // Assert
            realtimeDataAccess.DidNotReceive().PreprocessCustomerClassRealtime(Arg.Any<IEnumerable<CustClassificationRealtimeBySportEntity>>());
        }

        [Fact]
        public void PreprocessCustomerClassRealtime_WhenBatchSizeIsSmallerThanCustomers_ShouldCallPreprocessCustomerClassRealtimeMultipleTimes()
        {
            // Arrange
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<CustClassificationRealtimeBySportEntity>()
                {
                    new CustClassificationRealtimeBySportEntity { CustId = 1, SportGroup = 1 },
                    new CustClassificationRealtimeBySportEntity { CustId = 2, SportGroup = 2 },
                    new CustClassificationRealtimeBySportEntity { CustId = 3, SportGroup = 1 },
                    new CustClassificationRealtimeBySportEntity { CustId = 4, SportGroup = 2 }
                },
                ScannedMaxId = 7
            };

            var modelCategory = new GetCustomerCategoryBySportModel
            {
                CustomerNormals = new List<long> { 1, 2, 3, 4 },
                CustomerPADailyScans = new List<long>()
            };

            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);
            realtimeDataAccess.GetCustomerCategory(Arg.Any<string>()).Returns(modelCategory);

            // Act
            realtimeService.Start()
                .GetNewPlaceBetCustomers()
                .GetCustomerCategory(100)
                .PreprocessCustomerClassRealtime(2);

            // Assert
            realtimeDataAccess.Received(2).PreprocessCustomerClassRealtime(Arg.Any<IEnumerable<CustClassificationRealtimeBySportEntity>>());
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
