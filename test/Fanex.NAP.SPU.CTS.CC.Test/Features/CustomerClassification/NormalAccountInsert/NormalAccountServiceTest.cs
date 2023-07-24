namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class NormalAccountServiceTest
    {
        private readonly INormalAccountDataAccess normalAccountDataAccess;
        private readonly INormalAccountService normalAccountService;

        public NormalAccountServiceTest()
        {
            normalAccountDataAccess = Substitute.For<INormalAccountDataAccess>();
            normalAccountService = new NormalAccountService(normalAccountDataAccess, null);
        }

        [Fact]
        public void GetNormalAccounts_WhenSuccess()
        {
            // Arrange
           
            // Act
            normalAccountService.GetNormalAccounts();

            // Assert
            normalAccountDataAccess.Received(1).GetNormalAccount();
        }

        [Fact]
        public void GetCustomerAssociationProblemAccount_WhenSuccess()
        {
            // Arrange
            var custIDList = "1,2";

            // Act
            normalAccountService.GetCustomerAssociationProblemAccount(custIDList);

            // Assert
            normalAccountDataAccess.Received(1).GetCustsAssociationProblemAccount(custIDList);
        }

        [Fact]
        public void GetGroupBettingAndRejectCustomer_WhenSuccess()
        {
            // Arrange
            var custs = new List<GetNormalAccountEntity>()
            {
                new GetNormalAccountEntity { CustId = 1, ActiveDays = 1, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now },
                new GetNormalAccountEntity { CustId = 2, ActiveDays = 10, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now }
            };
            var custIds = "1,2";

            // Act
            normalAccountService.GetGroupBettingAndRejectCustomer(custs);

            // Assert
            normalAccountDataAccess.Received(1).CheckGroupBettingAndRejectCustomer(custIds);
        }

        [Fact]
        public void InsertNormalAccount_WhenSuccess_ReturnResult()
        {
            // Arrange
            var serviceName = "NormalAccountInsert";
            var custs = new List<InsertNormalAccountModel>()
            {
                new InsertNormalAccountModel(new NormalAccountAfterCheckTWDetectionModel(
                        new GetNormalAccountEntity{
                            CustId = 1, ActiveDays = 1, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now
                        }
                    )),
                new InsertNormalAccountModel(new NormalAccountAfterCheckTWDetectionModel(
                        new GetNormalAccountEntity{
                            CustId = 2, ActiveDays = 10, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now
                        }
                    ))
            };
            var custsJson = JsonConvert.SerializeObject(custs);
            var insertResult = new InsertNormalAccountResultEntity() { CustIds = new List<long> { 1, 2 }, ErrorMessage = string.Empty };

            // Act
            normalAccountDataAccess.InsertNormalAccount(custsJson).Returns(insertResult);

            normalAccountService.InsertNormalAccount(custs, serviceName, false);

            // Assert
            normalAccountDataAccess.Received(1).InsertNormalAccount(custsJson);
        }

        [Fact]
        public void UpdateCompleteNormalAccountInsert_WhenSuccess_ReturnNotException()
        {
            // Arrange
            var custIdList = "1, 2";
            var scannedMaxId = 10;

            // Act
            normalAccountService.UpdateCompleteNormalAccountInsert(custIdList, scannedMaxId, true);

            // Assert
            normalAccountDataAccess.Received(1).UpdateCompleteNormalAccountInsert(custIdList, scannedMaxId);
        }

    }
}
