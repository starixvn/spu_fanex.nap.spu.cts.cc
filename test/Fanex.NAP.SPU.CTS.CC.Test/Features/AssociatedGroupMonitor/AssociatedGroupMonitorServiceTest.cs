namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.AssociatedGroupMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using FluentAssertions;
    using NSubstitute;
    
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class AssociatedGroupMonitorServiceTest
    {
        private readonly IAssociatedGroupMonitorDataAccess associatedGroupMonitorDataAccess;
        private readonly IAssociatedGroupMonitorService associatedGroupMonitorService;
        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IBaseCustomerClassificationDataAccess baseCustomerClassificationDataAccess;
        private readonly IBaseCustomerClassificationService baseCustomerClassificationService;

        public AssociatedGroupMonitorServiceTest()
        {
            baseCustomerClassificationDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            baseCustomerClassificationService = new BaseCustomerClassificationService(baseCustomerClassificationDataAccess);
            
            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();           
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null);

            associatedGroupMonitorDataAccess = Substitute.For<IAssociatedGroupMonitorDataAccess>();
            associatedGroupMonitorService = new AssociatedGroupMonitorService(baseCustomerClassificationDataAccess,
                Substitute.For<IProblemAccountDataAccess>(), custClassToMainDBService, associatedGroupMonitorDataAccess);
           
        }

        [Fact]
        public void ScanAssociatedAccount__WhenSuccess_ReturnData()
        {
            // Arrange 

            // Act
            var assAccountModel = associatedGroupMonitorDataAccess.ScanAssociatedAccount();

            var result = associatedGroupMonitorService.ScanAssociatedAccount().Returns(assAccountModel);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void AddAssociatedAccount_WhenNotSuccess_ReturnNull()
        {
            // Arrange
            var model = ExpectedDataAddAssociatedAccountModel();

            // Act
            var result = associatedGroupMonitorService.AddAssociatedAccount(model);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddAssociatedAccount_WhenSuccess_ReturnNotNull()
        {
            // Arrange
            var model = ExpectedDataAddAssociatedAccountModel();

            // Act
            var data = associatedGroupMonitorDataAccess.AddAssociatedAccount(model);

            var result = associatedGroupMonitorService.AddAssociatedAccount(model).Returns(data);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetCustomerInfoMainDB_WhenSuccess_ReturnData()
        {
            // Arrange
            var custIDList = "1,2";
            var sourceType = 2;

            // Act
            var data = associatedGroupMonitorDataAccess.GetCustomerInfoMainDB(custIDList, sourceType);

            var result = associatedGroupMonitorService.GetCustomerInfoMainDB(custIDList, sourceType).Returns(data);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateABIToMainDB_WhenSuccess_ReturnNotNull()
        {
            // Arrange
            var valueAPI = 2;
            var dangerLevelModel = new List<DangerLevelModel>()
            {
                new DangerLevelModel
                {
                     CustId = 1, DangerLevel = valueAPI
                }
            };

            var customerInfoMainDBs = new List<CustomerInfoMainDBEntity>()
            {
                new CustomerInfoMainDBEntity
                {
                    CustID = 1, CustStatusID = 1, Danger1 =1, Danger2 = 0, Danger3 =2
                }
            };            

            // Act
            associatedGroupMonitorDataAccess.UpdateDangerLevelToMainDB(dangerLevelModel, 2);

            var result = associatedGroupMonitorService.UpdateABIToMainDB(customerInfoMainDBs, valueAPI);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ErrorCode);
        }

        [Fact]
        public void UpdateOriToMainDB_WhenSuccess_ReturnNotNull()
        {
            // Arrange
            var valueOri = 1;
            var dangerLevelModel = new List<DangerLevelModel>()
            {
                new DangerLevelModel
                {
                     CustId = 1, DangerLevel = valueOri
                }
            };

            var customerInfoMainDBs = new List<CustomerInfoMainDBEntity>()
            {
                new CustomerInfoMainDBEntity
                {
                    CustID = 1, CustStatusID = 1, Danger1 = 0, Danger2 = 2, Danger3 = 2
                }
            };

            // Act
            associatedGroupMonitorDataAccess.UpdateDangerLevelToMainDB(dangerLevelModel, 1);

            var result = associatedGroupMonitorService.UpdateOriToMainDB(customerInfoMainDBs, valueOri);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.ErrorCode);
        }

        [Fact]
        public void InsertProblemAccount_WhenSuccess_ReturnData()
        {
            // Arrange
            var pas = new List<InsertProblemAccountEntity>()
            {
                 new InsertProblemAccountEntity { CustID = 1, CTSCustID = 1, CategoryID = 30300, ActiveDays = 1, BetCount = 1 }
            };

            var pushDataParams = new PushDataToExternalEntity
            {
                ExternalBatchSize = 50,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = true,
                ServiceName = "Associated Group Monitor"
            };

            // Act

            baseCustomerClassificationDataAccess.When(x => x.InsertCustomerClassification(CCInputFlow.InsertPACategory, Arg.Any<InsertCustomerClassificationModel>()).Returns(new ResultInsertCustomerClassificationModel()));

            baseCustomerClassificationService.InsertCustomerClassification(CCInputFlow.InsertPACategory, Arg.Any<InsertCustomerClassificationModel>(), "").Returns(new ResultInsertCustomerClassificationModel());

            var result = associatedGroupMonitorService.InsertProblemAccount(pas, pushDataParams);

            // Assert
            Assert.NotNull(result);
        }

        private AddAssociatedAccountModel ExpectedDataAddAssociatedAccountModel()
        {
            return new AddAssociatedAccountModel()
            {
                CustIDs = "1,2",
                CTSCustIDs = "4,5",
                UserNames = "a,b",
                ABI = 1,
                IsAuto = false,
                GroupID = 1,
                CreatedBy = 8
            };
        }
     
    }
}
