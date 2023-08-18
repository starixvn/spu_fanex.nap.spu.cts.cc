namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.JobService;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;
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
        public void InsertNormalAccount_WhenSuccess_ReturnNotException()
        {
            // Arrange

            // Act
            normalAccountService.InsertNormalAccount(null, string.Empty, false);

            // Assert
            normalAccountDataAccess.Received(0).InsertNormalAccount(Arg.Any<string>());
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

        [Fact]
        public void MergeWithTWTaggedCust_WhenSuccess_ReturnResult()
        {
            // Arrange
            var cust = new List<GetNormalAccountEntity> { new GetNormalAccountEntity { CustId = 123, ActiveDays = 321 } };
            var twTaggedCust = new List<TWTaggedCustEntity> { new TWTaggedCustEntity { CustID = 123, TaggingID = 999 } };
            var compareResult = new List<NormalAccountAfterCheckTWDetectionModel> { new NormalAccountAfterCheckTWDetectionModel(cust[0], twTaggedCust[0]) };

            // Act
            var result = normalAccountService.MergeWithTWTaggedCust(cust, twTaggedCust);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithTWTaggedCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange

            // Act
            var result = normalAccountService.MergeWithTWTaggedCust(null, null);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MergeWithCTSTaggedCust_WhenSuccess_ReturnResult()
        {
            // Arrange
            var cust = new List<NormalAccountAfterCheckTWDetectionModel> { new NormalAccountAfterCheckTWDetectionModel(new GetNormalAccountEntity { CustId = 123, ActiveDays = 321 }, new TWTaggedCustEntity { CustID = 123, TaggingID = 999 }) };
            var twTaggedCust = new List<CTSTaggedCustEntity> { new CTSTaggedCustEntity { CustID = 123, TaggingID = 999, TaggingType = 123 } };
            var compareResult = new List<InsertNormalAccountModel> { new InsertNormalAccountModel(cust[0], twTaggedCust[0]) };

            // Act
            var result = normalAccountService.MergeWithCTSTaggedCust(cust, twTaggedCust);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithCTSTaggedCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange

            // Act
            var result = normalAccountService.MergeWithCTSTaggedCust(null, null);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void FlowMergeDataWithIsScanOnlyTagging_WhenSuccess_ReturnResult()
        {
            // Arrange
            var getnormalAccountList = new List<GetNormalAccountEntity>() {
                new GetNormalAccountEntity() { CustId = 1, IsScanOnlyTagging = true },
                new GetNormalAccountEntity() { CustId = 2, IsScanOnlyTagging = false },
                new GetNormalAccountEntity() { CustId = 3, IsScanOnlyTagging = true }
            };

            var twtTaggedCustsResult = new List<TWTaggedCustEntity>() { new TWTaggedCustEntity { CustID = 1, TaggingID = 999 } };

            var custAssWithPAReulst = new List<CTSTaggedCustEntity>() { new CTSTaggedCustEntity { CustID = 2, TaggingID = 222 } };

            var finalNormalAccJsonResult = "[{\"CustId\":1,\"SportGroupId\":0,\"CategoryId\":0,\"CreatedTime\":\"0001-01-01T00:00:00\",\"TurnoverRM\":0.0,\"WinlossRM\":0.0,\"BetCount\":0,\"ActiveDays\":0,\"PerformanceTime\":\"0001-01-01T00:00:00\",\"LastXDaysMargin\":0.0,\"ProbationPeriodWinloss\":0.0,\"TaggingID\":999,\"TaggingType\":0,\"TWBetCount\":0,\"TWGroupBettingRate\":0.0,\"TWTicketRejectRate\":0.0,\"TWDesktopUsageRate\":0.0,\"IsScanOnlyTagging\":true},{\"CustId\":2,\"SportGroupId\":0,\"CategoryId\":0,\"CreatedTime\":\"0001-01-01T00:00:00\",\"TurnoverRM\":0.0,\"WinlossRM\":0.0,\"BetCount\":0,\"ActiveDays\":0,\"PerformanceTime\":\"0001-01-01T00:00:00\",\"LastXDaysMargin\":0.0,\"ProbationPeriodWinloss\":0.0,\"TaggingID\":222,\"TaggingType\":0,\"TWBetCount\":0,\"TWGroupBettingRate\":0.0,\"TWTicketRejectRate\":0.0,\"TWDesktopUsageRate\":0.0,\"IsScanOnlyTagging\":false},{\"CustId\":3,\"SportGroupId\":0,\"CategoryId\":0,\"CreatedTime\":\"0001-01-01T00:00:00\",\"TurnoverRM\":0.0,\"WinlossRM\":0.0,\"BetCount\":0,\"ActiveDays\":0,\"PerformanceTime\":\"0001-01-01T00:00:00\",\"LastXDaysMargin\":0.0,\"ProbationPeriodWinloss\":0.0,\"TaggingID\":0,\"TaggingType\":0,\"TWBetCount\":0,\"TWGroupBettingRate\":0.0,\"TWTicketRejectRate\":0.0,\"TWDesktopUsageRate\":0.0,\"IsScanOnlyTagging\":true}]";

            // Act
            normalAccountDataAccess.CheckGroupBettingAndRejectCustomer(Arg.Any<string>()).Returns(twtTaggedCustsResult);

            normalAccountDataAccess.GetCustsAssociationProblemAccount(Arg.Any<string>()).Returns(custAssWithPAReulst);

            var twTaggedCusts = normalAccountService.GetGroupBettingAndRejectCustomer(getnormalAccountList);

            var custAfterCheckTWTagged = normalAccountService.MergeWithTWTaggedCust(getnormalAccountList, twTaggedCusts);

            var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = normalAccountService.GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

            var finalNormalAcc = normalAccountService.MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            // Assert
            normalAccountDataAccess.Received(1).CheckGroupBettingAndRejectCustomer(Arg.Any<string>());

            normalAccountDataAccess.Received(1).GetCustsAssociationProblemAccount(Arg.Any<string>());

            Assert.Equal(JsonConvert.SerializeObject(twtTaggedCustsResult), JsonConvert.SerializeObject(twTaggedCusts));

            Assert.Equal(JsonConvert.SerializeObject(custAssWithPAReulst), JsonConvert.SerializeObject(custAssWithPA));

            Assert.Equal(JsonConvert.SerializeObject(finalNormalAcc), finalNormalAccJsonResult);
        }
    }
}
