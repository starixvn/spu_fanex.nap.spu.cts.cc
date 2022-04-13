namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
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

        private readonly IDailyNormalClassificationDataAccess dailyClassificationDataAccess;
        private readonly IDailyNormalClassificationServices dailyClassificationServices;

        private readonly IFeedSoon88DataAccess feedSoon88DataAccess;
        private readonly IFeedSoon88Services feedSoon88Services;

        private readonly IAntiFraudCenterServices antiFraudCenterServices;

        private readonly ISystemParameterServices systemParameterService;

        private readonly IRobotAccountInsertServices robotAccountInsertServices;
        private readonly IRobotAccountInsertDataAccess robotAccountInsertDataAccess;

        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private readonly IDailyProblemClassificationDataAccess dailyProblemClassificationDataAccess;

        private readonly ICustomerEvidenceInsertServices customerEvidenceInsertServices;
        private readonly ICustomerEvidenceInsertDataAccess customerEvidenceInsertDataAccess;

        public RescanCustomerClassificationTest()
        {
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            problemAccountServices = new ProblemAccountServices(problemAccountDataAccess, null);

            feedSoon88DataAccess = Substitute.For<IFeedSoon88DataAccess>();
            feedSoon88Services = new FeedSoon88Services(feedSoon88DataAccess, null);

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null, feedSoon88Services);

            dailyClassificationDataAccess = Substitute.For<IDailyNormalClassificationDataAccess>();
            dailyClassificationServices = new DailyNormalClassificationServices(dailyClassificationDataAccess, systemParameterService);

            antiFraudCenterServices = new AntiFraudCenterServices(null);

            dailyProblemClassificationDataAccess = Substitute.For<IDailyProblemClassificationDataAccess>();
            dailyProblemClassificationServices = new DailyProblemClassificationServices(dailyProblemClassificationDataAccess, systemParameterService, problemAccountServices, custClassToMainDBService);

            customerEvidenceInsertDataAccess = Substitute.For<ICustomerEvidenceInsertDataAccess>();
            customerEvidenceInsertServices = new CustomerEvidenceInsertServices(customerEvidenceInsertDataAccess, problemAccountServices, dailyProblemClassificationServices, null);

            rescanCustClassificationDataAccess = Substitute.For<IRescanCustClassificationDataAccess>();
            rescanCustClassificationService = new RescanCustClassificationService(
                rescanCustClassificationDataAccess, null,
                problemAccountServices,
                antiFraudCenterServices,
                custClassToMainDBService,
                dailyClassificationServices,
                customerEvidenceInsertServices,
                dailyProblemClassificationServices);
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
                   RobotCounter = 1,
                   ReasonID = (int)Category.Robot
                },
                new ProblemAccountsEntity
                {
                   CustID = 2,
                   RobotCounter = 2,
                   ReasonID = (int)Category.Robot
                }
            };

            var robotUsers = new List<RobotUserEntity>
            {
                new RobotUserEntity
                {
                    CustId =1,
                    Counter = 1
                },
                new RobotUserEntity
                {
                    CustId = 2,
                    Counter = 2
                }
            };

            var custIds = new List<long> { 1, 2, 3 };

            rescanCustClassificationDataAccess.GetExistedRobotUser(string.Join(",", custIds)).Returns(robotUsers);

            rescanCustClassificationService.Start().GetRobotUsers(custIds);
            var result = rescanCustClassificationService.RobotUsers;

            result.Should().BeEquivalentTo(problemAccounts);
        }
    }
}