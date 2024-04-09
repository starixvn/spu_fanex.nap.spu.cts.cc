namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Consts;
    using Fanex.NAP.SPU.CTS.CC.Tests.Infrastructure.Extensions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

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
            var custs = new List<GetNormalAccountEntity>()
            {
                new GetNormalAccountEntity { CustId = 1, ActiveDays = 1, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now },
                new GetNormalAccountEntity { CustId = 2, ActiveDays = 10, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now }
            };
            var custIds = "1,2";

            var getGetGroupBettingAndRejectCustomerMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.GetGroupBettingAndRejectCustomer);

            // Act
            getGetGroupBettingAndRejectCustomerMethod.Invoke(normalAccountService, new object[] { custs });

            // Assert
            normalAccountDataAccess.Received(1).CheckGroupBettingAndRejectCustomer(custIds);
        }

        [Fact]
        public void GetNewTWGroupBetting_WhenSuccess()
        {
            // Arrange
            var custs = new List<GetNormalAccountEntity>()
            {
                new GetNormalAccountEntity { CustId = 1, ActiveDays = 1, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now },
                new GetNormalAccountEntity { CustId = 2, ActiveDays = 10, CategoryId = 1, BetCount = 1, PerformanceTime = DateTime.Now }
            };
            var custIds = "1,2";

            var getGetGroupBettingAndRejectCustomerMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.GetNewTWGroupBetting);

            // Act
            getGetGroupBettingAndRejectCustomerMethod.Invoke(normalAccountService, new object[] { custs });

            // Assert
            normalAccountDataAccess.Received(1).CheckNewGroupBetting(custIds);
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

            var getMergeWithTWTaggedCustMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.MergeWithTWTaggedCust);

            // Act
            var result = getMergeWithTWTaggedCustMethod.Invoke(normalAccountService, new object[] { cust, twTaggedCust });

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithTWTaggedCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange
            var getMergeWithTWTaggedCustMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.MergeWithTWTaggedCust);

            // Act
            var result = getMergeWithTWTaggedCustMethod.Invoke(normalAccountService, new object[] { null, null });

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

            var getMergeWithCTSTaggedCustMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.MergeWithCTSTaggedCust);

            // Act
            var result = getMergeWithCTSTaggedCustMethod.Invoke(normalAccountService, new object[] { cust, twTaggedCust });

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithCTSTaggedCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange
            var getMergeWithCTSTaggedCustMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.MergeWithCTSTaggedCust);

            // Act
            var result = getMergeWithCTSTaggedCustMethod.Invoke(normalAccountService, new object[] { null, null });

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MergeWithTWTaggedCustAndNewTWCust_WhenSuccess_ReturnResult()
        {
            // Arrange
            var cust = new List<GetNormalAccountEntity> {
                new GetNormalAccountEntity { CustId = 123, ActiveDays = 321 }, 
                new GetNormalAccountEntity { CustId = 124, ActiveDays = 321 }
            };
            var twTaggingSRPCusts = new List<TWTaggedCustEntity> { new TWTaggedCustEntity { CustID = 123, TaggingID = 999, TaggingType = 123 } };
            var twGBTaggingNewCusts = new List<NewTWTaggedCustEntity> { new NewTWTaggedCustEntity { CustID = 124, TaggingID = 1000, TaggingType = 124, SportType = 333 } };
            var compareResult = new List<NormalAccountAfterCheckTWDetectionModel> {
                new NormalAccountAfterCheckTWDetectionModel(cust[0], twTaggingSRPCusts[0]),
                new NormalAccountAfterCheckTWDetectionModel(cust[1], twGBTaggingNewCusts[0])
            };

            var getMergeWithCTSTaggedCustMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.MergeWithTWTaggedCustAndNewTWCust);

            // Act
            var result = getMergeWithCTSTaggedCustMethod.Invoke(normalAccountService, new object[] { cust, twTaggingSRPCusts, twGBTaggingNewCusts });

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(result), JsonConvert.SerializeObject(compareResult));
        }

        [Fact]
        public void MergeWithTWTaggedCustAndNewTWCust_WhenSuccess_ReturnNotNull()
        {
            // Arrange

            var getMergeWithCTSTaggedCustMethod = TestExtension.GetPrivateMethod(typeof(NormalAccountService), NormalAccountInsertConsts.MergeWithTWTaggedCustAndNewTWCust);

            // Act
            var result = getMergeWithCTSTaggedCustMethod.Invoke(normalAccountService, new object[] { null, null, null});

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void MergeWithTWTaggedCustAndCTSTaggedCust_WhenSuccess()
        {
            // Arrange
            var getnormalAccountList = new List<GetNormalAccountEntity>() {
                new GetNormalAccountEntity() { CustId = 1, ScanTaggingTWType = 2, ScanTaggingType = (int)NormalClasification_TaggingType.NotExist, CategoryId = 201 },
                new GetNormalAccountEntity() { CustId = 2, ScanTaggingTWType = 1, ScanTaggingType = (int)NormalClasification_TaggingType.NotExist },
                new GetNormalAccountEntity() { CustId = 3, ScanTaggingTWType = 0, ScanTaggingType = (int)NormalClasification_TaggingType.ExistOnly }
            };

            var twTaggingSRPCusts = new List<TWTaggedCustEntity>() { new TWTaggedCustEntity { CustID = 1, TaggingID = 999 } };

            var twGBTaggingNewCusts = new List<NewTWTaggedCustEntity>() { new NewTWTaggedCustEntity { CustID = 3, TaggingID = 456 } };

            var custAssWithPAReulst = new List<CTSTaggedCustEntity>() { new CTSTaggedCustEntity { CustID = 2, TaggingID = 222 } };

            // Act
            normalAccountDataAccess.CheckNewGroupBetting(Arg.Any<string>()).Returns(twGBTaggingNewCusts);

            normalAccountDataAccess.CheckGroupBettingAndRejectCustomer(Arg.Any<string>()).Returns(twTaggingSRPCusts);

            normalAccountDataAccess.GetCustsAssociationProblemAccount(Arg.Any<string>()).Returns(custAssWithPAReulst);

            var finalNormalAcc = normalAccountService.MergeWithTWTaggedCustAndCTSTaggedCust(getnormalAccountList);

            // Assert
            normalAccountDataAccess.Received(1).CheckNewGroupBetting(Arg.Any<string>());

            normalAccountDataAccess.Received(1).CheckGroupBettingAndRejectCustomer(Arg.Any<string>());

            normalAccountDataAccess.Received(1).GetCustsAssociationProblemAccount(Arg.Any<string>());

            Assert.NotNull(finalNormalAcc);
        }

        [Fact]
        public void MergeWithTaggedNormalTaggingFlow_WhenSuccess()
        {
            // Arrange
            var getnormalAccountList = new List<GetNormalAccountEntity>() {
                new GetNormalAccountEntity() { CustId = 1, ScanTaggingTWType = 0, ScanTaggingType = (int)NormalClasification_TaggingType.NotExist, CategoryId = 201 },
                new GetNormalAccountEntity() { CustId = 2, ScanTaggingTWType = 1, ScanTaggingType = (int)NormalClasification_TaggingType.NotExist },
                new GetNormalAccountEntity() { CustId = 3, ScanTaggingTWType = 0, ScanTaggingType = (int)NormalClasification_TaggingType.ExistOnly }
            };

            var twTaggingSRPCusts = new List<TWTaggedCustEntity>() { new TWTaggedCustEntity { CustID = 1, TaggingID = 999 } };

            var custAssWithPAReulst = new List<CTSTaggedCustEntity>() { new CTSTaggedCustEntity { CustID = 2, TaggingID = 222 } };

            // Act
            normalAccountDataAccess.CheckGroupBettingAndRejectCustomer(Arg.Any<string>()).Returns(twTaggingSRPCusts);

            normalAccountDataAccess.GetCustsAssociationProblemAccount(Arg.Any<string>()).Returns(custAssWithPAReulst);

            var finalNormalAcc = normalAccountService.MergeWithTaggedNormalTaggingFlow(getnormalAccountList);

            // Assert
            normalAccountDataAccess.Received(1).CheckGroupBettingAndRejectCustomer(Arg.Any<string>());

            normalAccountDataAccess.Received(1).GetCustsAssociationProblemAccount(Arg.Any<string>());

            Assert.NotNull(finalNormalAcc);
        }
    }
}
