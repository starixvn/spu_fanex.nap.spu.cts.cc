namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassificationBySport.NormalAccountBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.BaseCustomerClassificationBySport;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class NormalAccountBySportServiceTest
    {
        private readonly INormalAccountInsertBySportDataAccess normalAccountDataAccess;
        private readonly INormalAccountInsertBySportService normalAccountService;
        private readonly IBaseCustomerClassificationBySportDataAccess baseCustomerClassificationBySportDataAccess;

        public NormalAccountBySportServiceTest()
        {
            normalAccountDataAccess = Substitute.For<INormalAccountInsertBySportDataAccess>();
            baseCustomerClassificationBySportDataAccess = Substitute.For<IBaseCustomerClassificationBySportDataAccess>();
            normalAccountService = new NormalAccountInsertBySportService(baseCustomerClassificationBySportDataAccess, normalAccountDataAccess, null);
        }

        [Fact]
        public void GetNormalAccount_WhenSuccess_ReturnResult()
        {
            // Arrange

            // Act
            normalAccountService.GetNormalClassificationBySport();

            // Assert
            normalAccountDataAccess.Received(1).GetNormalClasificationAccount();
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
            var result = new ResultInsertClassificationBySportModel();

            // Act
            normalAccountService.InsertNormalClassificationBySport(custs, serviceName).Returns(result);

            normalAccountService.InsertNormalClassificationBySport(custs, serviceName);

            // Assert
            normalAccountService.Received(1).InsertNormalClassificationBySport(custs, serviceName);
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

