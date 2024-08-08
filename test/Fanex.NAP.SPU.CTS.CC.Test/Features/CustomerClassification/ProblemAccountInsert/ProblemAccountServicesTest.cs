namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class ProblemAccountServicesTest
    {
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IBaseCustomerClassificationDataAccess baseDataAccess;
        private readonly ISentryClient sentryClient;

        public ProblemAccountServicesTest()
        {
            sentryClient = Substitute.For<ISentryClient>();
            baseDataAccess = Substitute.For<IBaseCustomerClassificationDataAccess>();

            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            problemAccountServices = new ProblemAccountServices(baseDataAccess, problemAccountDataAccess, sentryClient);
        }
      
        [Fact]
        public void GetProblemAccountLosingClassification_WhenSuccess_ReturnData()
        {
            // Arrange
            var custIdList = new List<long> { 1, 2, 3 };
            var funcId = FunctionId.InsertPAJob;

            // Act
            var result = problemAccountServices.GetProblemAccountLosingClassification(custIdList, funcId);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<ProblemAccountLosingClassificationEntity>>(result);
        }

        [Fact]
        public void RescanCustomerClassification__WhenSuccess_ReturnData()
        {
            // Arrange            
            var funcId = FunctionId.InsertPAJob;
            var custRescans = new List<GetRescanCustomerClassificationEntity>()
            {
                new GetRescanCustomerClassificationEntity {  CustID = 1, CategoryID = 1 , CTSCustID = 1}
            };
            var ccInputFlow = CCInputFlow.InsertPACategory;
            var isToQueue = false;
            var serviceName = "Test";
            var expectedInsertResult = new ResultInsertCustomerClassificationModel()
            {
                CustInfos = new List<CustInfoClassificationEntity> { new CustInfoClassificationEntity { CustID = 1 } }
            };

            // Act
            baseDataAccess.InsertCustomerClassification(ccInputFlow, Arg.Any<InsertCustomerClassificationModel>()).Returns(expectedInsertResult);

            var result = problemAccountServices.RescanCustomerClassification(custRescans, funcId, ccInputFlow, isToQueue, serviceName);
            
            // Assert
            result.Should().NotBeNull();    
        }

        [Fact]
        public void InsertPALogics_WhenSuccess_ReturnNotException()
        {
            // Arrange 
            var custInfo = new List<CustInfoEntity>()
            {
                new CustInfoEntity {  CustID = 1, CTSCustID = 1 }
            };

            var custIdsToString = string.Join(",", custInfo.Select(x=>x.CustID));

            // Act
            problemAccountDataAccess.UpdateBetLimitOfFlaggedCustomer(custIdsToString).Returns(new DBResult());

            Action action = () => problemAccountServices.InsertPALogics(custInfo, "TWProblemAccountInsert");

            // Assert
            action.Should().NotThrow<Exception>();
        }       

        [Fact]
        public void RemoveProblemAccountPackage_WhenSuccess_ReturnNotException()
        {
            // Arrange 
            var problemAccounts = new List<RemoveProblemAccountEntity>()
            {
                new RemoveProblemAccountEntity { CustID = 1 }
            };
            var isFromQueue = false;
            var ccInputFlow = CCInputFlow.RemovePACategory;
            var serviceName = "Test";
            var expectedRemoveResult = new ResultRemoveCustomerClassificationModel()
            {
                CustInfos = new List<ResultRemoveCustomerClassificationEntity>() { new ResultRemoveCustomerClassificationEntity {  CustID = 1} }
            };

            // Act
            baseDataAccess.RemoveCustomerClassification(ccInputFlow, Arg.Any<RemoveCustomerClassificationModel>()).Returns(expectedRemoveResult);

            Action action = () => problemAccountServices.RemoveProblemAccountPackage(problemAccounts, isFromQueue, serviceName);

            // Assert
            action.Should().NotThrow<Exception>();
        }
    }
}
