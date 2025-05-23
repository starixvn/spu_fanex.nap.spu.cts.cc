namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Model;
    using Xunit;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using static Fanex.NAP.SPU.CTS.Shares.IEnumerableExtention;

    public class NormalAccountServiceTest
    {
        private readonly INormalAccountDataAccess normalAccountDataAccess;
        private readonly INormalAccountService normalAccountService;

        public NormalAccountServiceTest()
        {
            normalAccountDataAccess = Substitute.For<INormalAccountDataAccess>();
            normalAccountService = new NormalAccountService(Substitute.For<IBaseCustomerClassificationDataAccess>(), normalAccountDataAccess, null);
        }

        [Fact]
        public void GetNormalAccounts_WhenSuccess()
        {
            // Arrange

            // Act
            normalAccountService.GetNormalAccounts(false);

            // Assert
            normalAccountDataAccess.Received(1).GetNormalAccount(false);
        }

        [Fact]
        public void GetCustomerAssociationProblemAccount_WhenSuccess()
        {
            // Arrange
            var custIDList = "1,2";

            var getCustomerAssociationProblemAccountMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.GetCustomerAssociationProblemAccount);

            // Act
            getCustomerAssociationProblemAccountMethod.Invoke(normalAccountService, new object[] { custIDList });

            // Assert
            normalAccountDataAccess.Received(1).GetCustsAssociationProblemAccount(custIDList);
        }

        [Fact]
        public void GetGroupBettingAndRejectCustomer_WhenSuccess()
        {
            // Arrange
            var custs = new List<ProcessTaggingModel>()
            {
                new ProcessTaggingModel { CustId = 1, ActiveDays = 1, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now },
                new ProcessTaggingModel { CustId = 2, ActiveDays = 10, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now }
            };
            var custIds = "1,2";

            var getGetGroupBettingAndRejectCustomerMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.GetGroupBettingAndRejectCustomer);

            // Act
            getGetGroupBettingAndRejectCustomerMethod.Invoke(normalAccountService, new object[] { custs });

            // Assert
            normalAccountDataAccess.Received(1).CheckGroupBettingAndRejectCustomer(custIds);
        }

        [Fact]
        public void UpdateCompleteNormalAccountInsert_WhenSuccess_ReturnNotException()
        {
            // Arrange
            long scannedMaxId = 10;

            // Act
            normalAccountService.CompleteNormalAccountInsert(scannedMaxId, true);

            // Assert
            normalAccountDataAccess.Received(1).CompleteNormalAccountInsert(scannedMaxId);
        }

        [Fact]
        public void MergeWithTWTaggedCust_WhenSuccess_ReturnResult()
        {
            // Arrange
            var cust = new List<ProcessTaggingModel> { new ProcessTaggingModel { CustId = 123, ActiveDays = 321 } };
            var twTaggedCust = new List<TWTaggedCustEntity> { new TWTaggedCustEntity { CustID = 123, TaggingID = 999 } };
            var compareResult = new List<ProcessTaggingModel> { cust[0].MergeTWTaggedCust(twTaggedCust[0]) };

            // Act
            var result = MergeWithOtherEnumerable(cust, twTaggedCust, (source, other) => source.MergeTWTaggedCust(other));

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithTWTaggedCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange

            // Act
            var result = MergeWithOtherEnumerable<ProcessTaggingModel, TWTaggedCustEntity>(null, null, (source, other) => source.MergeTWTaggedCust(other));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MergeWithCTSTaggedCust_WhenSuccess_ReturnResult()
        {
            // Arrange
            var cust = new List<ProcessTaggingModel> { new ProcessTaggingModel { CustId = 123, ActiveDays = 321, TaggingID = 0 } };
            var twTaggedCust = new List<CTSTaggedCustEntity> { new CTSTaggedCustEntity { CustID = 123, TaggingID = 999, TaggingType = 123 } };
            var compareResult = new List<ProcessTaggingModel> { cust[0].MergeCTSTaggedCust(twTaggedCust[0]) };

            // Act
            var result = MergeWithOtherEnumerable(cust, twTaggedCust, (source, other) => source.MergeCTSTaggedCust(other));

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithCTSTaggedCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange

            // Act
            var result = MergeWithOtherEnumerable<ProcessTaggingModel, CTSTaggedCustEntity>(null, null, (source, other) => source.MergeCTSTaggedCust(other));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MergeWithTWTaggedSpecialLicSubCust_WhenSuccess_ReturnResult()
        {
            // Arrange
            var cust = new List<ProcessTaggingModel> {
                new ProcessTaggingModel { CustId = 123, ActiveDays = 321, TaggingID = 0, ScanSpecialLicSubType = 1 },
                new ProcessTaggingModel { CustId = 321, ActiveDays = 321, TaggingID = 999, ScanSpecialLicSubType = 0 }
            };
            var twTaggedCust = new List<CTSTaggedCustEntity> { new CTSTaggedCustEntity { CustID = 123, TaggingID = 999, TaggingType = 123 } };
            var compareResult = new List<ProcessTaggingModel> { cust[0].MergeTWTaggedSpecialLicSubCust(twTaggedCust[0]), cust[1] };

            // Act
            var result = MergeWithOtherEnumerable(cust, twTaggedCust, (source, other) => source.MergeTWTaggedSpecialLicSubCust(other));

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithTWTaggedSpecialLicSubCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange

            // Act
            var result = MergeWithOtherEnumerable<ProcessTaggingModel, CTSTaggedCustEntity>(null, null, (source, other) => source.MergeTWTaggedSpecialLicSubCust(other));

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MergeWithTaggedNormalTaggingFlow_WhenSuccess()
        {
            // Arrange
            var getnormalAccountList = new List<GetNormalAccountEntity>()
            {
                new GetNormalAccountEntity
                {
                    CustId = 1,
                    ScanTaggingTWType = 1,
                    ScanTaggingType = (int)NormalClasification_TaggingType.NotExist,
                    CategoryId = 201
                },
                new GetNormalAccountEntity
                {
                    CustId = 2,
                    ScanTaggingTWType = 1,
                    ScanTaggingType = (int)NormalClasification_TaggingType.NotExist,
                    CategoryId = 202
                },
                new GetNormalAccountEntity
                {
                    CustId = 3,
                    ScanTaggingTWType = 0,
                    ScanTaggingType = (int)NormalClasification_TaggingType.ExistOnly,
                    CategoryId = 203
                },
                new GetNormalAccountEntity
                {
                    CustId = 4,
                    ScanTaggingTWType = 0,
                    ScanTaggingType = (int)NormalClasification_TaggingType.ExistOnly,
                    CategoryId = 203,
                    ScanSpecialLicSubType = 1
                }
            };

            var twTaggedCusts = new List<TWTaggedCustEntity>()
            {
                new TWTaggedCustEntity
                {
                    CustID = 1,
                    TaggingID = 999,
                    TaggingType = 1,
                    TWBetCount = 5,
                    TWGroupBettingRate = 0.8m,
                    TWTicketRejectRate = 0.2m,
                    TWDesktopUsageRate = 0.5m
                },
                new TWTaggedCustEntity
                {
                    CustID = 2,
                    TaggingID = 888,
                    TaggingType = 2,
                    TWBetCount = 3,
                    TWGroupBettingRate = 0.6m,
                    TWTicketRejectRate = 0.1m,
                    TWDesktopUsageRate = 0.4m
                }
            };

            var excludedCustIds = new List<TWTaggedExcludeEntity> { new TWTaggedExcludeEntity { CustID = 1 } };

            var custAssWithPAResult = new List<CTSTaggedCustEntity>()
            {
                new CTSTaggedCustEntity
                {
                    CustID = 2,
                    TaggingID = 222,
                    TaggingType = 3
                }
            };

            var specialLicSubTaggedCusts = new List<CTSTaggedCustEntity>()
            {
                new CTSTaggedCustEntity
                {
                    CustID = 4,
                    TaggingID = 777,
                    TaggingType = 5
                }
            };

            // Act
            normalAccountDataAccess.CheckGroupBettingAndRejectCustomer(Arg.Any<string>()).Returns(twTaggedCusts);
            normalAccountDataAccess.CheckTWTaggingExclude(Arg.Any<IEnumerable<TWTaggedExcludeModel>>()).Returns(excludedCustIds);
            normalAccountDataAccess.GetCustsAssociationProblemAccount(Arg.Any<string>()).Returns(custAssWithPAResult);
            normalAccountDataAccess.GetTWTaggedCustBySpecialLicSub(Arg.Any<string>()).Returns(specialLicSubTaggedCusts);

            var result = normalAccountService.MergeWithTaggedNormalTaggingFlow(getnormalAccountList);

            // Assert
            normalAccountDataAccess.Received(1).CheckGroupBettingAndRejectCustomer(Arg.Is<string>(x => x == "1,2"));

            normalAccountDataAccess.Received(1).CheckTWTaggingExclude(Arg.Is<IEnumerable<TWTaggedExcludeModel>>(
                models => models.Any(m =>
                    m.CustID == 1 && m.CategoryID == 201 && m.TaggingID == 999) &&
                models.Any(m =>
                    m.CustID == 2 && m.CategoryID == 202 && m.TaggingID == 888)));

            normalAccountDataAccess.Received(1).GetCustsAssociationProblemAccount(Arg.Any<string>());

            normalAccountDataAccess.Received(1).GetTWTaggedCustBySpecialLicSub(Arg.Any<string>());

            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.NotEmpty(resultList);

            // Verify excluded customer is included in result with correct tagging
            var excludedCust = resultList.FirstOrDefault(x => x.CustID == 1);
            Assert.NotNull(excludedCust);
            Assert.Equal(999, excludedCust.TaggingID);
            Assert.Equal(1, excludedCust.TaggingType);
            Assert.Equal(5, excludedCust.TWBetCount);
            Assert.Equal(0.8m, excludedCust.TWGroupBettingRate);
            Assert.Equal(0.2m, excludedCust.TWTicketRejectRate);
            Assert.Equal(0.5m, excludedCust.TWDesktopUsageRate);
        }

        [Fact]
        public void MergeWithTaggedNormalTaggingFlow_WhenInputListIsNull_ReturnEmptyList()
        {
            // Act
            var result = normalAccountService.MergeWithTaggedNormalTaggingFlow(null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            normalAccountDataAccess.DidNotReceive().CheckGroupBettingAndRejectCustomer(Arg.Any<string>());
            normalAccountDataAccess.DidNotReceive().CheckTWTaggingExclude(Arg.Any<IEnumerable<TWTaggedExcludeModel>>());
            normalAccountDataAccess.DidNotReceive().GetCustsAssociationProblemAccount(Arg.Any<string>());
            normalAccountDataAccess.DidNotReceive().GetTWTaggedCustBySpecialLicSub(Arg.Any<string>());
        }

        [Fact]
        public void MergeWithTaggedNormalTaggingFlow_WhenInputListIsEmpty_ReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<GetNormalAccountEntity>();

            // Act
            var result = normalAccountService.MergeWithTaggedNormalTaggingFlow(emptyList);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            normalAccountDataAccess.DidNotReceive().CheckGroupBettingAndRejectCustomer(Arg.Any<string>());
            normalAccountDataAccess.DidNotReceive().CheckTWTaggingExclude(Arg.Any<IEnumerable<TWTaggedExcludeModel>>());
            normalAccountDataAccess.DidNotReceive().GetCustsAssociationProblemAccount(Arg.Any<string>());
            normalAccountDataAccess.DidNotReceive().GetTWTaggedCustBySpecialLicSub(Arg.Any<string>());
        }
    }
}
