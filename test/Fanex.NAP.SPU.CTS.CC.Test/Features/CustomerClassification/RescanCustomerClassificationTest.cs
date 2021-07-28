namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.DailyClassification;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.FeedCCToSoon88;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public class RescanCustomerClassificationTest
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly IRescanCustClassificationService rescanCustClassificationService;

        private readonly IProblemAccountDataAccess problemAccountDataAccess;
        private readonly IProblemAccountServices problemAccountServices;

        private readonly ICustClassToMainDBDataAccess custClassToMainDBDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        private readonly IDailyClassificationDataAccess dailyClassificationDataAccess;
        private readonly IDailyClassificationServices dailyClassificationServices;

        private readonly IFeedSoon88DataAccess feedSoon88DataAccess;
        private readonly IFeedSoon88Services feedSoon88Services;

        public RescanCustomerClassificationTest()
        {
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            problemAccountServices = new ProblemAccountServices(problemAccountDataAccess, null);

            feedSoon88DataAccess = Substitute.For<IFeedSoon88DataAccess>();
            feedSoon88Services = new FeedSoon88Services(feedSoon88DataAccess, null);

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, feedSoon88Services);

            dailyClassificationDataAccess = Substitute.For<IDailyClassificationDataAccess>();
            dailyClassificationServices = new DailyClassificationServices(dailyClassificationDataAccess);

            rescanCustClassificationDataAccess = Substitute.For<IRescanCustClassificationDataAccess>();
            rescanCustClassificationService = new RescanCustClassificationService(
                rescanCustClassificationDataAccess,null,
                problemAccountServices,custClassToMainDBService,
                dailyClassificationServices);
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

            var custIds = new List<long>{ 1,2,3 };

            var custTORescanPA = new List<long> { 2 };

            rescanCustClassificationDataAccess.GetRoleCustomer(string.Join(",", custIds)).Returns(roleCusts);

            rescanCustClassificationService.Start().GetAgentCusts(custIds);
            var result = rescanCustClassificationService.CustIdsToRescanPA;

            result.Should().BeEquivalentTo(custTORescanPA);
        }

        [Fact]
        public void GetExistedEvidences_WhenSuccess_ReturnCustEvidences()
        {
            var custEvidences = new List<CustEvidenceEntity>
            {
                new CustEvidenceEntity
                {
                    CTSCustId = 1,
                    CustId = 1,
                    EvidenceId = 1,
                    ActionType = 1,
                    SubscriberId = 1
                },
                new CustEvidenceEntity
                {
                    CTSCustId = 2,
                    CustId = 2,
                    EvidenceId = 2,
                    ActionType = 2,
                    SubscriberId = 2
                }
            };

            var custIds = new List<long> { 1, 2, 3 };

            string custIdsToString = string.Join(",", custIds);
            rescanCustClassificationDataAccess.GetExistedEvidences(custIdsToString).Returns(custEvidences);

            rescanCustClassificationService.Start().GetExistedCustEvidences(custIds);
            var result = rescanCustClassificationService.ExistedEvidences;

            result.Should().BeEquivalentTo(custEvidences);
        }

        [Fact]
        public void GetRobotUser_WhenSuccess_ReturnProblemAccounts()
        {
            var problemAccounts = new List<ProblemAccountsEntity>
            {
                new ProblemAccountsEntity
                {
                   CustID = 1,
                   ReasonID = (int)Category.Robot
                },
                new ProblemAccountsEntity
                {
                   CustID = 2,
                   ReasonID = (int)Category.Robot
                },
                new ProblemAccountsEntity
                {
                   CustID = 3,
                   ReasonID = (int)Category.Robot
                }
            };

            var custIds = new List<long> { 1, 2, 3 };

            string custIdsToString = string.Join(",", custIds);
            rescanCustClassificationDataAccess.GetCustByRobotSiteRule(custIdsToString).Returns(custIds);
            rescanCustClassificationDataAccess.GetExistedRobotUser(string.Join(",", custIds)).Returns(custIdsToString);

            rescanCustClassificationService.Start().GetRobotUsers(custIds);
            var result = rescanCustClassificationService.RobotUsers;

            result.Should().BeEquivalentTo(problemAccounts);
        }

        [Fact]
        public void SyncRobotUserToUpdateCategory_WhenSuccess_ReturnProblemAccounts()
        {
            var custIds = new List<long> { 1, 2, 3 };

            string custIdsToString = string.Join(",", custIds);
            rescanCustClassificationDataAccess.GetCustByRobotSiteRule(custIdsToString).Returns(custIds);
            rescanCustClassificationDataAccess.GetExistedRobotUser(string.Join(",", custIds)).Returns(custIdsToString);

            var result = rescanCustClassificationService.Start().SyncRobotUserToUpdateCategory(custIds);

            result.Should().BeEquivalentTo(custIds);
        }
    }
}