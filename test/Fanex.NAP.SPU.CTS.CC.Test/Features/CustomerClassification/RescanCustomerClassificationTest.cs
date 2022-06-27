namespace Fanex.NAP.SPU.CTS.Tests.Features.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.FeedCCToSoon88;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using Xunit;

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

        private readonly IRobotTWInsertServices robotAccountInsertServices;
        private readonly IRobotTWInsertDataAccess robotAccountInsertDataAccess;

        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private readonly IDailyProblemClassificationDataAccess dailyProblemClassificationDataAccess;

        private readonly IDataLogTrackingServices dataLogTrackingServices;

        public RescanCustomerClassificationTest()
        {
            problemAccountDataAccess = Substitute.For<IProblemAccountDataAccess>();
            problemAccountServices = new ProblemAccountServices(problemAccountDataAccess, null);

            feedSoon88DataAccess = Substitute.For<IFeedSoon88DataAccess>();
            feedSoon88Services = new FeedSoon88Services(feedSoon88DataAccess, null);

            custClassToMainDBDataAccess = Substitute.For<ICustClassToMainDBDataAccess>();
            custClassToMainDBService = new CustClassToMainDBService(custClassToMainDBDataAccess, null,  feedSoon88Services, null);

            dailyClassificationDataAccess = Substitute.For<IDailyNormalClassificationDataAccess>();
            dailyClassificationServices = new DailyNormalClassificationServices(dailyClassificationDataAccess, systemParameterService);

            dailyProblemClassificationDataAccess = Substitute.For<IDailyProblemClassificationDataAccess>();
            dailyProblemClassificationServices = new DailyProblemClassificationServices(dailyProblemClassificationDataAccess, systemParameterService, problemAccountServices, custClassToMainDBService);

            rescanCustClassificationDataAccess = Substitute.For<IRescanCustClassificationDataAccess>();
            rescanCustClassificationService = new RescanCustClassificationService(
                rescanCustClassificationDataAccess, null,
                problemAccountServices,
                custClassToMainDBService,
                dailyClassificationServices,
                dataLogTrackingServices);
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
   
    }
}