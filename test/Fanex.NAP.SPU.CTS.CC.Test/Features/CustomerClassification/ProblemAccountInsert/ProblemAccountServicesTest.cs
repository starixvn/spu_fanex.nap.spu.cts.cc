namespace Fanex.NAP.SPU.CTS.CC.Tests.Features.CustomerClassification.ProblemAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class ProblemAccountServicesTest
    {
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;

        public ProblemAccountServicesTest()
        {
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            problemAccountServices = new ProblemAccountServices(Substitute.For<IBaseCustomerClassificationDataAccess>(), problemAccountDataAccess, null);
        }

        [Fact]
        public void GetExistedProblemAccounts_WhenSuccess_ReturnData()
        {
            // Arrange
            //var custIdList = new List<long> { 1, 2, 3 };

            //// Act
            //var result = problemAccountServices.GetExistedProblemAccounts(custIdList);

            //// Assert
            //Assert.IsAssignableFrom<IEnumerable<ExistedProblemAccountEntity>>(result);
        }

        [Fact]
        public void GetProblemAccountLosingClassification_WhenSuccess_ReturnData()
        {
            // Arrange
            var custIdList = new List<long> { 1, 2, 3 };

            // Act
            var result = problemAccountServices.GetProblemAccountLosingClassification(custIdList, FunctionId.InsertPAJob);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<ProblemAccountLosingClassificationEntity>>(result);
        }

        [Fact]
        public void InsertProblemAccountPackage__WhenSuccess_ReturnData()
        {
            //// Arrange  
            //var expectedResult = new List<long>();
            //var pa = new List<InsertProblemAccountEntity>()
            //{
            //    new InsertProblemAccountEntity
            //    {  
            //        CustID =1, CTSCustID =1, CategoryID = 158, CategoryGroupID = 158
            //    } 
            //};

            //// Act
            //var result = problemAccountServices.InsertProblemAccountPackage(pa, FunctionId.TWProblemAccountInsert
            //    , InsertPA_ByType.Reason
            //    , false
            //    , "TWProblemAccountInsert");

            //// Assert
            //result.Should().NotBeNull();
            //result.Should().BeEquivalentTo(expectedResult);
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
        public void RemovePALogics_WhenSuccess_ReturnNotException()
        {
            //// Arrange 
            //var custInfo = new List<CustRemovePAEntity>()
            //{
            //    new CustRemovePAEntity {  CustID = 1, CTSCustID = 1 }
            //};

            //// Act
            //Action action = () => problemAccountServices.RemovePALogics(custInfo, "TWProblemAccountInsert");

            //// Assert
            //action.Should().NotThrow<Exception>();
        }

        [Fact]
        public void RemoveOldProblemAccount_WhenSuccess_ReturnNotException()
        {
            // Arrange 
            //var custIdList = new List<long>() { 1, 2, 3 };

            //// Act
            //Action action = () => problemAccountServices.RemoveOldProblemAccount(custIdList);

            //// Assert
            //action.Should().NotThrow<Exception>();
        }
    }
}
