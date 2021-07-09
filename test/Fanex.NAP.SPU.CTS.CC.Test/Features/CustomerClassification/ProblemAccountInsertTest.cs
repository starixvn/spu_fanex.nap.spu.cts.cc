namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Shares;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ProblemAccountInsertTest
    {
        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;

        public ProblemAccountInsertTest()
        {
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();

            problemAccountServices = new ProblemAccountServices(problemAccountDataAccess, null);
        }

        [Fact]
        public void GetProblemAccountsFromCN88_WhenSuccess_ReturnProblemAccountList()
        {
            var problemAccounts = new List<ProblemAccountsEntity> {
                new ProblemAccountsEntity
                {
                  CustID = 1,
                  ReasonID=1
                },
                 new ProblemAccountsEntity
                {
                      CustID = 2,
                  ReasonID=2
                },
                  new ProblemAccountsEntity
                {
                     CustID = 3,
                  ReasonID=3
                },
            };

            problemAccountDataAccess.GetProblemAccounts_CN88(Constants.ProblemAccountByDateTime, null).Returns(problemAccounts);

            var result = problemAccountServices.GetProblemAccounts_CN88(Constants.ProblemAccountByDateTime, null);

            result.Should().BeEquivalentTo(problemAccounts);
        }

        [Fact]
        public void GetProblemAccountsFromIBC_WhenSuccess_ReturnProblemAccountList()
        {
            var problemAccounts = new List<ProblemAccountsEntity> {
                new ProblemAccountsEntity
                {
                  CustID = 1,
                  ReasonID=1
                },
                 new ProblemAccountsEntity
                {
                      CustID = 2,
                  ReasonID=2
                },
                  new ProblemAccountsEntity
                {
                     CustID = 3,
                  ReasonID=3
                },
            };

            problemAccountDataAccess.GetProblemAccounts_IBC(Constants.ProblemAccountByDateTime, null).Returns(problemAccounts);

            var result = problemAccountServices.GetProblemAccounts_IBC(Constants.ProblemAccountByDateTime, null);

            result.Should().BeEquivalentTo(problemAccounts);
        }

        [Fact]
        public void GetProblemAccounts_WhenSuccess_ReturnProblemAccountList()
        {
            var result = problemAccountServices.GetProblemAccounts(DateTime.Now, "ProblemAccountInsert", false);

            result.Should().NotBeNull();
        }

        [Fact]
        public void InsertProblemAccounts_WhenSuccess_ReturnCustIds()
        {
            var problemAccounts = new List<ProblemAccountsEntity> {
                new ProblemAccountsEntity
                {
                  CustID = 1,
                  ReasonID=1
                },
                 new ProblemAccountsEntity
                {
                      CustID = 2,
                  ReasonID=2
                },
                  new ProblemAccountsEntity
                {
                     CustID = 3,
                  ReasonID=3
                },
            };

            var dataResult = new InsertCategoryResultEntity
            {
                ErrorMessage = string.Empty,
                CustIds = new List<long>() { 1, 2, 3, 4, 5 }
            };

            var problemAccountJson = JsonConvert.SerializeObject(problemAccounts);

            problemAccountDataAccess.InsertProblemAccount(problemAccountJson, false).Returns(dataResult);

            var result = problemAccountServices.InsertProblemAccountToCTS(problemAccounts, false, "ProblemAccountInsert", false);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(dataResult.CustIds);
        }
    }
}