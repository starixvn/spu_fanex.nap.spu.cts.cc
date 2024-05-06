namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class RealtimeClassificationServiceTest
    {
        private readonly IRealtimeClassificationDataAccess realtimeDataAccess;
        private readonly IRealtimeClassificationService realtimeService;

        public RealtimeClassificationServiceTest()
        {
            realtimeDataAccess = Substitute.For<IRealtimeClassificationDataAccess>();
            realtimeService = new RealtimeClassificationService(realtimeDataAccess, null);
        }

        [Fact]
        public void GetNewPlaceBetCustomers_WhenSuccess()
        {
            // Arrange

            // Act
            realtimeService.GetNewPlaceBetCustomers();

            // Assert
            realtimeDataAccess.Received(1).GetNewPlaceBetCustomers();
        }

        [Fact]
        public void GetCustomerCategory_ReturnNotException()
        {
            // Arrange
            var custIDList = "1,2";            
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<GetNewPlaceBetCustomerEntity>() {
                    new GetNewPlaceBetCustomerEntity {  CustId = 1, SportId = 1},
                    new GetNewPlaceBetCustomerEntity{  CustId = 2, SportId = 1}
                },
                ScannedMaxId = 10
            };
            var modelCustCategory = new GetCustomerCategoryRealtimeModel()
            {
                CustomerCategory = new List<long>() { 1, 2 },
                CustomerPADailyScan = new List<long>()
            };

            // Act
            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);

            realtimeDataAccess.GetCustomerCategory(custIDList).Returns(modelCustCategory);

            realtimeService.Start().GetNewPlaceBetCustomers().GetCustomerCategory(5000);            

            // Assert
            realtimeDataAccess.Received(1).GetCustomerCategory(custIDList);

        }

        [Fact]
        public void PreprocessCustomerClassRealtime_WhenSuccess()
        {
            // Arrange
            var custIDList = "1,2";
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<GetNewPlaceBetCustomerEntity>() {
                    new GetNewPlaceBetCustomerEntity { CustId = 1, SportId = 1 },
                    new GetNewPlaceBetCustomerEntity { CustId = 2, SportId = 1 }
                },
                ScannedMaxId = 10
            };
            var modelCustCategory = new GetCustomerCategoryRealtimeModel()
            {
                CustomerCategory = new List<long> { 1, 2 }
            };

            // Act
            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);

            realtimeDataAccess.GetCustomerCategory(custIDList).Returns(modelCustCategory);

            realtimeService.Start().GetNewPlaceBetCustomers()
                .GetCustomerCategory(5000)
                .PreprocessCustomerClassRealtime(5000);

            // Assert
            realtimeDataAccess.Received(1).PreprocessCustomerClassRealtime(custIDList);

        }

        [Fact]
        public void InsertPADailyScanQueue_WhenSuccess()
        {
            // Arrange
            var custIDList = "1,2";
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<GetNewPlaceBetCustomerEntity>() {
                    new GetNewPlaceBetCustomerEntity {  CustId = 1, SportId = 1},
                    new GetNewPlaceBetCustomerEntity{  CustId = 2, SportId = 1}
                },
                ScannedMaxId = 10
            };
            var modelCustCategory = new GetCustomerCategoryRealtimeModel()
            {
                CustomerCategory = new List<long>(),                  
                CustomerPADailyScan = new List<long>() { 1, 2 }
            };

            // Act
            realtimeDataAccess.GetNewPlaceBetCustomers().Returns(modelCustomers);

            realtimeDataAccess.GetCustomerCategory(custIDList).Returns(modelCustCategory);

            realtimeService.Start().GetNewPlaceBetCustomers()
                .GetCustomerCategory(5000)
                .InsertPADailyScanQueue(5000);

            // Assert
            realtimeDataAccess.Received(1).InsertPADailyScanQueue(custIDList);
        }

        [Fact]
        public void CompleteCustomerRealtime_WhenSuccess()
        {
            // Arrange
            var modelCustomers = new GetNewPlaceBetCustomerModel()
            {
                CustomerNewPlaceBets = new List<GetNewPlaceBetCustomerEntity>() {
                    new GetNewPlaceBetCustomerEntity {  CustId = 1, SportId = 1},
                    new GetNewPlaceBetCustomerEntity{  CustId = 2, SportId = 1}
                },
                ScannedMaxId = 10
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
