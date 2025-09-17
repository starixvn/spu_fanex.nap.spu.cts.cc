using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Entities;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Model;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Fanex.NAP.SPU.CTS.Shares.Constants;
using Fanex.NAP.SPU.CTS.Shares.Models;

namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.RealtimeTWInitialSmartBySportService
{
    public class TWInitialSmartBySportServiceTest
    {
        private readonly IBaseCustomerClassificationDataAccess _baseDataAccess;
        private readonly IProblemAccountDataAccess _problemAccountDataAccess;
        private readonly ITWInitialSmartBySportDataAccess _dataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly ISystemParameterServices _systemParameter;
        private readonly TWInitialSmartBySportService _service;

        public TWInitialSmartBySportServiceTest()
        {
            _baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();
            _problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            _dataAccess = Substitute.For<ITWInitialSmartBySportDataAccess>();
            _custClassToMainDBService = Substitute.For<ICustClassToMainDBService>();
            _systemParameter = Substitute.For<ISystemParameterServices>();

            _service = new TWInitialSmartBySportService(
                _baseDataAccess,
                _problemAccountDataAccess,
                _dataAccess,
                _custClassToMainDBService,
                _systemParameter);
        }

        [Fact]
        public void Start_ShouldInitializeTWSmartCustData_ReturnsService()
        {
            // Act
            var result = _service.Start();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            Assert.Same(_service, result);
        }

        [Fact]
        public void GetCustInitialSmartBySport_WithValidBatchSize_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var batchSize = 100;
            var expectedModel = new TWInitialSmartBySportModel
            {
                CustInfos = new List<TWInitialSmartBySportEntity>
                {
                    new TWInitialSmartBySportEntity { CustID = 1, CTSCustID = 1001 }
                },
                NextScannedCustID = 1000,
                NextScannedDateTime = DateTime.Now
            };

            _dataAccess.GetCustInitialSmartBySport(batchSize).Returns(expectedModel);

            // Act
            var result = _service.GetCustInitialSmartBySport(batchSize);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            Assert.Same(_service, result);
            _dataAccess.Received(1).GetCustInitialSmartBySport(batchSize);
        }

        [Fact]
        public void InsertTWInitialSmartClassification_WithEmptyCustInfos_ShouldNotProcess_ReturnsService()
        {
            // Arrange
            var parameters = new PushDataToExternalEntity
            {
                ExternalBatchSize = 50,
                ServiceName = "TestService"
            };

            // Set up empty data
            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>())
                      .Returns(new TWInitialSmartBySportModel { CustInfos = new List<TWInitialSmartBySportEntity>() });

            _service.GetCustInitialSmartBySport(100);

            // Act
            var result = _service.InsertTWInitialSmartClassification(parameters);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            _custClassToMainDBService.DidNotReceive().PushExternalPackageByCustId(Arg.Any<IEnumerable<long>>(), Arg.Any<PushDataToExternalEntity>());
        }

        [Fact]
        public void UpdateLastProcessed_WithEmptyCustInfos_ShouldReturnFalse()
        {
            // Arrange - Set up empty data
            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>())
                      .Returns(new TWInitialSmartBySportModel { CustInfos = new List<TWInitialSmartBySportEntity>() });

            _service.GetCustInitialSmartBySport(100);

            // Act
            var result = _service.UpdateLastProcessed();

            // Assert
            Assert.False(result);
            _systemParameter.DidNotReceive().UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>());
        }

        [Fact]
        public void UpdateLastProcessed_WithValidCustInfos_ShouldUpdateSystemParameters_ReturnsTrue()
        {
            // Arrange
            var testDateTime = new DateTime(2023, 12, 1, 10, 30, 45);
            var testCustId = 12345L;
            var custInfos = new List<TWInitialSmartBySportEntity>
            {
                new TWInitialSmartBySportEntity { CustID = 1, CTSCustID = 1001 }
            };

            var model = new TWInitialSmartBySportModel
            {
                CustInfos = custInfos,
                NextScannedCustID = testCustId,
                NextScannedDateTime = testDateTime
            };

            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>()).Returns(model);
            _systemParameter.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>()).Returns(true);

            // Set up the service with data
            _service.GetCustInitialSmartBySport(100);

            // Act
            var result = _service.UpdateLastProcessed();

            // Assert
            Assert.True(result);
            _systemParameter.Received(1).UpdateTimingExecution(
                testCustId.ToString(),
                CTS_SystemParameters.TWInitialSmartBySportClassification_LastCustID);
            _systemParameter.Received(1).UpdateTimingExecution(
                testDateTime.ToString(CONVERT_TO_SQL_DATETIME),
                CTS_SystemParameters.TWInitialSmartBySportClassification_LastScanDate);
        }

        [Fact]
        public void GetCustInitialSmartBySport_WithZeroBatchSize_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var batchSize = 0;
            var expectedModel = new TWInitialSmartBySportModel();

            _dataAccess.GetCustInitialSmartBySport(batchSize).Returns(expectedModel);

            // Act
            var result = _service.GetCustInitialSmartBySport(batchSize);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            _dataAccess.Received(1).GetCustInitialSmartBySport(batchSize);
        }

        [Fact]
        public void GetCustInitialSmartBySport_WithNegativeBatchSize_ShouldCallDataAccess_ReturnsService()
        {
            // Arrange
            var batchSize = -10;
            var expectedModel = new TWInitialSmartBySportModel();

            _dataAccess.GetCustInitialSmartBySport(batchSize).Returns(expectedModel);

            // Act
            var result = _service.GetCustInitialSmartBySport(batchSize);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            _dataAccess.Received(1).GetCustInitialSmartBySport(batchSize);
        }

        [Fact]
        public void InsertTWInitialSmartClassification_WithValidCustInfos_ShouldProcessAndPushData_ReturnsService()
        {
            // Arrange
            var custInfos = new List<TWInitialSmartBySportEntity>
            {
                new TWInitialSmartBySportEntity
                {
                    CustID = 1,
                    CTSCustID = 1001,
                    RoleID = 1,
                    SubscriberID = 1,
                    CategoryID = 1,
                    CategoryGroup = 1,
                    IsLicensee = true,
                    SportType = 1,
                    DetectedDate = DateTime.Now,
                    Probability = 0.8m
                },
                new TWInitialSmartBySportEntity
                {
                    CustID = 2,
                    CTSCustID = 1002,
                    RoleID = 2,
                    SubscriberID = 2,
                    CategoryID = 2,
                    CategoryGroup = 2,
                    IsLicensee = false,
                    SportType = 2,
                    DetectedDate = DateTime.Now,
                    Probability = 0.9m
                }
            };

            var parameters = new PushDataToExternalEntity
            {
                ExternalBatchSize = 1, // Force chunking
                ServiceName = "TestService"
            };

            // Mock the base data access to return a valid result for InsertCustomerClassification
            var insertResult = new ResultInsertCustomerClassificationModel
            {
                CustInfos = new List<CustInfoClassificationEntity>
                {
                    new CustInfoClassificationEntity
                    {
                        CustID = 1,
                        CTSCustID = 1001,
                        CustomerClass = 1,
                        SportID = 1,
                        CategoryID = 1,
                        ScannedTime = DateTime.Now,
                        IsPA = true,
                        IsScanDgrAssociation = false
                    },
                    new CustInfoClassificationEntity
                    {
                        CustID = 2,
                        CTSCustID = 1002,
                        CustomerClass = 2,
                        SportID = 2,
                        CategoryID = 2,
                        ScannedTime = DateTime.Now,
                        IsPA = true,
                        IsScanDgrAssociation = false
                    }
                },
                ErrorMessage = null
            };

            _baseDataAccess.InsertCustomerClassification(
                Arg.Any<CCInputFlow>(),
                Arg.Any<InsertCustomerClassificationModel>())
                .Returns(insertResult);

            // Mock the problem account data access methods
            _problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(Arg.Any<string>())
                .Returns(new DBResult { ErrorCode = 0, ErrorMsg = null });

            _problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(Arg.Any<string>())
                .Returns(0);

            _custClassToMainDBService.PushExternalPackageByCustId(
                Arg.Any<IEnumerable<long>>(),
                Arg.Any<PushDataToExternalEntity>())
                .Returns(true);

            // Set up the service with data
            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>())
                      .Returns(new TWInitialSmartBySportModel { CustInfos = custInfos });

            _service.GetCustInitialSmartBySport(100);

            // Act
            var result = _service.InsertTWInitialSmartClassification(parameters);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            // Note: InsertProblemAccountPackage is called on the service itself (inherited from base class)
            // We can only verify the external service calls
            _custClassToMainDBService.Received(2).PushExternalPackageByCustId(
                Arg.Any<IEnumerable<long>>(),
                parameters);
        }

        [Fact]
        public void InsertTWInitialSmartClassification_WithSingleChunk_ShouldProcessOnce_ReturnsService()
        {
            // Arrange
            var custInfos = new List<TWInitialSmartBySportEntity>
            {
                new TWInitialSmartBySportEntity
                {
                    CustID = 1,
                    CTSCustID = 1001,
                    RoleID = 1,
                    SubscriberID = 1,
                    CategoryID = 1,
                    CategoryGroup = 1,
                    IsLicensee = true,
                    SportType = 1,
                    DetectedDate = DateTime.Now,
                    Probability = 0.8m
                }
            };

            var parameters = new PushDataToExternalEntity
            {
                ExternalBatchSize = 10, // Large enough to not chunk
                ServiceName = "TestService"
            };

            // Mock the base data access to return a valid result for InsertCustomerClassification
            var insertResult = new ResultInsertCustomerClassificationModel
            {
                CustInfos = new List<CustInfoClassificationEntity>
                {
                    new CustInfoClassificationEntity
                    {
                        CustID = 1,
                        CTSCustID = 1001,
                        CustomerClass = 1,
                        SportID = 1,
                        CategoryID = 1,
                        ScannedTime = DateTime.Now,
                        IsPA = true,
                        IsScanDgrAssociation = false
                    }
                },
                ErrorMessage = null
            };

            _baseDataAccess.InsertCustomerClassification(
                Arg.Any<CCInputFlow>(),
                Arg.Any<InsertCustomerClassificationModel>())
                .Returns(insertResult);

            // Mock the problem account data access methods
            _problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(Arg.Any<string>())
                .Returns(new DBResult { ErrorCode = 0, ErrorMsg = null });

            _problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(Arg.Any<string>())
                .Returns(0);

            _custClassToMainDBService.PushExternalPackageByCustId(
                Arg.Any<IEnumerable<long>>(),
                Arg.Any<PushDataToExternalEntity>())
                .Returns(true);

            // Set up the service with data
            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>())
                      .Returns(new TWInitialSmartBySportModel { CustInfos = custInfos });

            _service.GetCustInitialSmartBySport(100);

            // Act
            var result = _service.InsertTWInitialSmartClassification(parameters);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            // Note: InsertProblemAccountPackage is called on the service itself (inherited from base class)
            _custClassToMainDBService.Received(1).PushExternalPackageByCustId(
                Arg.Any<IEnumerable<long>>(),
                parameters);
        }

        [Fact]
        public void UpdateLastProcessed_WithSystemParameterFailure_ShouldStillReturnTrue()
        {
            // Arrange
            var testDateTime = new DateTime(2023, 12, 1, 10, 30, 45);
            var testCustId = 12345L;
            var custInfos = new List<TWInitialSmartBySportEntity>
            {
                new TWInitialSmartBySportEntity { CustID = 1, CTSCustID = 1001 }
            };

            var model = new TWInitialSmartBySportModel
            {
                CustInfos = custInfos,
                NextScannedCustID = testCustId,
                NextScannedDateTime = testDateTime
            };

            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>()).Returns(model);
            _systemParameter.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>()).Returns(false);

            // Set up the service with data
            _service.GetCustInitialSmartBySport(100);

            // Act
            var result = _service.UpdateLastProcessed();

            // Assert
            Assert.True(result); // Method always returns true regardless of system parameter update result
            _systemParameter.Received(1).UpdateTimingExecution(
                testCustId.ToString(),
                CTS_SystemParameters.TWInitialSmartBySportClassification_LastCustID);
            _systemParameter.Received(1).UpdateTimingExecution(
                testDateTime.ToString(CONVERT_TO_SQL_DATETIME),
                CTS_SystemParameters.TWInitialSmartBySportClassification_LastScanDate);
        }

        [Fact]
        public void FluentInterface_ShouldAllowMethodChaining()
        {
            // Arrange
            var parameters = new PushDataToExternalEntity
            {
                ExternalBatchSize = 50,
                ServiceName = "TestService"
            };

            var model = new TWInitialSmartBySportModel
            {
                CustInfos = new List<TWInitialSmartBySportEntity>
                {
                    new TWInitialSmartBySportEntity { CustID = 1, CTSCustID = 1001 }
                },
                NextScannedCustID = 1000,
                NextScannedDateTime = DateTime.Now
            };

            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>()).Returns(model);

            // Mock the base data access to return a valid result for InsertCustomerClassification
            var insertResult = new ResultInsertCustomerClassificationModel
            {
                CustInfos = new List<CustInfoClassificationEntity>
                {
                    new CustInfoClassificationEntity
                    {
                        CustID = 1,
                        CTSCustID = 1001,
                        CustomerClass = 1,
                        SportID = 1,
                        CategoryID = 1,
                        ScannedTime = DateTime.Now,
                        IsPA = true,
                        IsScanDgrAssociation = false
                    }
                },
                ErrorMessage = null
            };

            _baseDataAccess.InsertCustomerClassification(
                Arg.Any<CCInputFlow>(),
                Arg.Any<InsertCustomerClassificationModel>())
                .Returns(insertResult);

            // Mock the problem account data access methods
            _problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(Arg.Any<string>())
                .Returns(new DBResult { ErrorCode = 0, ErrorMsg = null });

            _problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(Arg.Any<string>())
                .Returns(0);

            _custClassToMainDBService.PushExternalPackageByCustId(
                Arg.Any<IEnumerable<long>>(),
                Arg.Any<PushDataToExternalEntity>())
                .Returns(true);

            _systemParameter.UpdateTimingExecution(Arg.Any<string>(), Arg.Any<CTS_SystemParameters>()).Returns(true);

            // Act
            var result = _service
                .Start()
                .GetCustInitialSmartBySport(100)
                .InsertTWInitialSmartClassification(parameters)
                .UpdateLastProcessed();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void InsertTWInitialSmartClassification_WithLargeDataset_ShouldHandleChunkingCorrectly()
        {
            // Arrange
            var custInfos = new List<TWInitialSmartBySportEntity>();
            for (int i = 1; i <= 25; i++) // Create 25 entities
            {
                custInfos.Add(new TWInitialSmartBySportEntity
                {
                    CustID = i,
                    CTSCustID = 1000 + i,
                    RoleID = i,
                    SubscriberID = i,
                    CategoryID = i,
                    CategoryGroup = i,
                    IsLicensee = i % 2 == 0,
                    SportType = i,
                    DetectedDate = DateTime.Now,
                    Probability = 0.5m + (i * 0.01m)
                });
            }

            var parameters = new PushDataToExternalEntity
            {
                ExternalBatchSize = 10, // Should create 3 chunks: 10, 10, 5
                ServiceName = "TestService"
            };

            // Mock the base data access to return a valid result for InsertCustomerClassification
            var insertResult = new ResultInsertCustomerClassificationModel
            {
                CustInfos = custInfos.Select(x => new CustInfoClassificationEntity
                {
                    CustID = x.CustID,
                    CTSCustID = x.CTSCustID,
                    CustomerClass = 1,
                    SportID = x.SportType,
                    CategoryID = x.CategoryID,
                    ScannedTime = DateTime.Now,
                    IsPA = true,
                    IsScanDgrAssociation = false
                }).ToList(),
                ErrorMessage = null
            };

            _baseDataAccess.InsertCustomerClassification(
                Arg.Any<CCInputFlow>(),
                Arg.Any<InsertCustomerClassificationModel>())
                .Returns(insertResult);

            // Mock the problem account data access methods
            _problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(Arg.Any<string>())
                .Returns(new DBResult { ErrorCode = 0, ErrorMsg = null });

            _problemAccountDataAccess.CheckNewPAAssociationCustomerForRollback(Arg.Any<string>())
                .Returns(0);

            _custClassToMainDBService.PushExternalPackageByCustId(
                Arg.Any<IEnumerable<long>>(),
                Arg.Any<PushDataToExternalEntity>())
                .Returns(true);

            // Set up the service with data
            _dataAccess.GetCustInitialSmartBySport(Arg.Any<int>())
                      .Returns(new TWInitialSmartBySportModel { CustInfos = custInfos });

            _service.GetCustInitialSmartBySport(100);

            // Act
            var result = _service.InsertTWInitialSmartClassification(parameters);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TWInitialSmartBySportService>(result);
            // Note: InsertProblemAccountPackage is called on the service itself (inherited from base class)
            // We can only verify the external service calls - should be called 3 times for 3 chunks
            _custClassToMainDBService.Received(3).PushExternalPackageByCustId(
                Arg.Any<IEnumerable<long>>(),
                parameters);
        }
    }
}
