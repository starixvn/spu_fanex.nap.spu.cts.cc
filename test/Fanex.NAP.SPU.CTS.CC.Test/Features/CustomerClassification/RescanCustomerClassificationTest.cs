namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.PAPotentialClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.FeedCCToSoon88;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RescanCustomerClassificationTest
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly IRescanCustClassificationService rescanCustClassificationService;

        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;

        private readonly IPAPotentialClassificationDataAccess paPotentialDataAccess;
        private readonly IPAPotentialClassificationService paPotentialServices;

        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private readonly IFeedSoon88DataAccess feedSoon88DataAccess;
        private readonly IFeedSoon88Services feedSoon88Services;

        public RescanCustomerClassificationTest()
        {   
            //problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            //problemAccountServices = new ProblemAccountServices(Substitute.For<IBaseCustomerClassificationDataAccess>(), problemAccountDataAccess, null);

            //paPotentialDataAccess = Substitute.For<IPAPotentialClassificationDataAccess>();
            //paPotentialServices = new PAPotentialClassificationService(paPotentialDataAccess);

            //feedSoon88DataAccess = Substitute.For<IFeedSoon88DataAccess>();
            //feedSoon88Services = new FeedSoon88Services(feedSoon88DataAccess, null);

            //custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            //custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, feedSoon88Services, null);

            //rescanCustClassificationDataAccess = Substitute.For<IRescanCustClassificationDataAccess>();
            //rescanCustClassificationService = new RescanCustClassificationService(
            //    rescanCustClassificationDataAccess,
            //    paPotentialDataAccess,
            //    null,
            //    null,
            //    problemAccountServices,
            //    custClassToMainDBService,
            //    null);
        }

        [Fact]
        public void GetRoleCustomer_WhenSuccess_ReturnCustList()
        {
            var roleCusts = new List<RoleCustomerEntity>
            {
                new RoleCustomerEntity
                {
                   CustID = 1,
                   RoleID = 1
                },
                  new RoleCustomerEntity
                {
                   CustID = 2,
                   RoleID = 3
                },
            };

            var custIds = new List<long> { 1, 2, 3 };

            rescanCustClassificationDataAccess.GetRoleCustomer(string.Join(",", custIds)).Returns(roleCusts);

            rescanCustClassificationService.Start().GetAgentCusts(custIds);

            var result = rescanCustClassificationService.CustIdsToRescanPA;

            Assert.IsAssignableFrom<IEnumerable<long>>(result);
        }

        [Fact]
        public void GetExistedProblemAccounts_WhenSuccess_ReturnData()
        {
            //var existedPA = CreateSampleExistedPAModel();

            //var custIds = new List<long> { 1, 2, 3 };

            //var expectPA = CreateSampleProblemAccountModel();
            //var expectRobot = CreateSampleRobotModel();

            //problemAccountDataAccess.GetExistedProblemAccounts(string.Join(",", custIds)).Returns(existedPA);
            //rescanCustClassificationService.Start().GetExistedProblemAccounts(custIds);

            //var paResult = rescanCustClassificationService.ExistedPA;
            //var rebotResult = rescanCustClassificationService.ExistedRobot;

            //paResult.Should().BeEquivalentTo(expectPA);
            //rebotResult.Should().BeEquivalentTo(expectRobot);
        }

        [Fact]
        public void InsertProblemAccounts_WhenSuccess_ReturnData()
        {
            //var existedPA = CreateSampleExistedPAModel();

            //var problemAccount = new List<InsertProblemAccountEntity>
            //{
            //    CreateSampleProblemAccountModel()
            //};

            //var custIds = new List<long> { 1, 2, 3 };

            //problemAccountDataAccess.GetExistedProblemAccounts(string.Join(",", custIds)).Returns(existedPA);

            //Action action = () => rescanCustClassificationService
            //                        .Start()
            //                        .GetExistedProblemAccounts(custIds)
            //                        .InsertProblemAccountLogics(null, false, FunctionId.InsertPAJob);

            //action.Should().NotThrow<Exception>();
        }


        [Fact]
        public void GetTWRobots_WhenSuccess_ReturnData()
        {
            var expectData = new List<InsertRobotEntity>
            {
                CreateSampleTWRobotModel()
            };

            var inputData = new List<TWRobotUserEntity>
            {
                new TWRobotUserEntity { CustId = 0, Counter = 0}
            };

            var custIds = new List<long> { 1, 2, 3 };

            rescanCustClassificationDataAccess.GetExistedTWRobotUser(string.Join(",", custIds)).Returns(inputData);

            rescanCustClassificationService
                                    .Start()
                                    .GetTWRobotUsers(custIds);

            var robots = rescanCustClassificationService.TWRobots;

            robots.Should().BeEquivalentTo(expectData);
        }

        [Fact]
        public void InsertRobots_WhenSuccess_ReturnData()
        {
            //var expectData = new List<InsertRobotEntity>
            //{
            //    CreateSampleTWRobotModel()
            //};

            //var inputData = new List<TWRobotUserEntity>
            //{
            //    new TWRobotUserEntity { CustId = 0, Counter = 0}
            //};

            //var custIds = new List<long> { 1, 2, 3 };

            //rescanCustClassificationDataAccess.GetExistedTWRobotUser(string.Join(",", custIds)).Returns(inputData);

            //Action action = () =>  rescanCustClassificationService
            //                                .Start()
            //                                .GetTWRobotUsers(custIds)
            //                                .InsertRobot();


            //action.Should().NotThrow<Exception>();

        }

        private List<ExistedProblemAccountEntity> CreateSampleExistedPAModel()
        {
            var rd = new Random();
            var data = new List<ExistedProblemAccountEntity>
            {
                new ExistedProblemAccountEntity
                {
                   CustID = 0,
                   CTSCustID = 0,
                   CategoryID = 0,
                   LastModifiedDate = DateTime.MinValue,
                   IsRobot = false
                },
                new ExistedProblemAccountEntity
                {
                   CustID = 0,
                   CTSCustID = 0,
                   CategoryID = 0,
                   LastModifiedDate = DateTime.MinValue,
                   IsRobot = true
                }
            };

            return data;
        }

        private InsertRobotEntity CreateSampleRobotModel()
        {
            var rd = new Random();
            var data = new InsertRobotEntity
            {
                CustID = 0,
                IsFromTW = false,
                IsFromCTS = false,
                IsDirect = false,
                IsFromAI = false,
                IsFromTVS = false,
                RobotCounter = null,
                CreatedBy = StarixITId,
                Remark = null,
                BetCount = null,
                TVSRequestID = null,
                ActiveDays = null,
                TurnoverRM = null,
                WinlossRM = null,
                WinlossStatus = null
            };

            return data;
        }

        private InsertRobotEntity CreateSampleTWRobotModel()
        {
            var rd = new Random();
            var data = new InsertRobotEntity
            {
                CustID = 0,
                RobotCounter = 0,
                IsFromTW = true,
                IsDirect = true,
                CreatedBy = StarixITId
            };

            return data;
        }

        private InsertProblemAccountEntity CreateSampleProblemAccountModel()
        {
            var rd = new Random();
            var data = new InsertProblemAccountEntity
            {
                CustID = 0,
                CTSCustID = 0,
                CategoryID = 0,
                LastModifiedDate = DateTime.MinValue,
                CreatedBy = StarixITId,
                IsDirectPA = false
            };

            return data;
        }
    }
}