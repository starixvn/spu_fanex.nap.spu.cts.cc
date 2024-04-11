namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class NormalAccountBySportServiceTest
    {
        private readonly INormalAccountInsertBySportDataAccess normalAccountDataAccess;
        private readonly INormalAccountInsertBySportService normalAccountService;

        public NormalAccountBySportServiceTest()
        {
            normalAccountDataAccess = Substitute.For<INormalAccountInsertBySportDataAccess>();
            normalAccountService = new NormalAccountInsertBySportService(normalAccountDataAccess, null);
        }

        [Fact]
        public void GetNormalAccount_WhenSuccess_ReturnResult()
        {
            // Arrange

            // Act
            normalAccountService.GetNormalAccount();

            // Assert
            normalAccountDataAccess.Received(1).GetNormalAccount();
        }

        [Fact]
        public void InsertNormalAccount_WhenSuccess_ReturnResult()
        {
            // Arrange
            var serviceName = "NormalAccountInsertBySport";
            var custs = new List<NormalAccountBySportEntity>()
            {
                new NormalAccountBySportEntity { CustId = 1, ActiveDays = 1, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now },
                new NormalAccountBySportEntity { CustId = 2, ActiveDays = 7, CategoryId = 1, BetCount = 10, PerformanceTime = DateTime.Now },
            };
            var custsJson = JsonConvert.SerializeObject(custs);
            var insertResult = new InsertNormalAccountBySportModel();

            // Act
            normalAccountDataAccess.InsertNormalAccount(custsJson).Returns(insertResult);

            normalAccountService.InsertNormalAccount(custs, serviceName);

            // Assert
            normalAccountDataAccess.Received(1).InsertNormalAccount(custsJson);
        }

        [Fact]
        public void CompleteNormalAccountBySport_WhenSuccess_ReturnTrue()
        {
            // Arrange                        
            long scannedMaxId = 7;

            // Act
            normalAccountService.CompleteNormalAccountBySport(scannedMaxId, true);

            // Assert
            normalAccountDataAccess.Received(1).CompleteNormalAccountBySport(scannedMaxId);
        }

    }
}

