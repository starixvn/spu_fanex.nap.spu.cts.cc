namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.TWRobot;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Shares;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotTWInsertServices
    {
        IRobotTWInsertServices Start();

        IRobotTWInsertServices GetLastTimespan();

        IRobotTWInsertServices GetTWRobotUser();

        IRobotTWInsertServices GetCustomerRole();

        IRobotTWInsertServices GetRobotUserCategoryId(string serviceName);

        bool InsertTWRobotPackage(int internalSize, string agencyApiUrl, PushDataToExternalEntity pushConfig);      

        void UpdateLastTimespan();

        TWRobotUserViewEntity RobotUserInfo { get; }
    }

    public class RobotTWInsertServices : ProblemAccountServices, IRobotTWInsertServices
    {
        private readonly IRobotTWInsertDataAccess _robotAccountInsertDataAccess;
        private readonly ISystemParameterServices _systemParameterServices;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly IDataLogTrackingServices _dataLogTrackingServices;
        private readonly ICustomerClassificationAgencyService _ccAgencyService;
        private readonly ISentryClient _sentryClient;
        private long lastScannedTimespan;
        private TWRobotUserViewEntity robotInfo;
        private int? robotCategoryId;

        public RobotTWInsertServices(
            IRobotTWInsertDataAccess robotAccountInsertDataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IDataLogTrackingServices dataLogTrackingServices,
            IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustomerClassificationAgencyService ccAgencyService,
            ISentryClient sentryClient) : base(baseDataAccess, problemAccountDataAccess, sentryClient)

        {
            this._robotAccountInsertDataAccess = robotAccountInsertDataAccess;
            this._systemParameterServices = systemParameterServices;
            this._custClassToMainDBService = custClassToMainDBService;
            this._dataLogTrackingServices = dataLogTrackingServices;
            this._ccAgencyService = ccAgencyService;
            this._sentryClient = sentryClient;
        }

        public IRobotTWInsertServices Start()
        {
            lastScannedTimespan = 0;

            robotInfo = new TWRobotUserViewEntity
            {
                RobotUsers = new List<TWRobotUserEntity>(),
                MaxTimestamp = 0
            };

            return this;
        }

        public IRobotTWInsertServices GetLastTimespan()
        {
            lastScannedTimespan = _systemParameterServices.GetLastScannedId(CTS_SystemParameters.RobotAccountLastScannedTime);

            return this;
        }

        public IRobotTWInsertServices GetTWRobotUser()
        {
            robotInfo = _robotAccountInsertDataAccess.GetTWRobotUsers(lastScannedTimespan);

            return this;
        }

        public IRobotTWInsertServices GetCustomerRole()
        {
            if (robotInfo != null && robotInfo.RobotUsers != null && robotInfo.RobotUsers.Any())
            {
                var custIDs = string.Join(",", robotInfo.RobotUsers.Select(x => x.CustId).Distinct());

                var dicCustRole = _robotAccountInsertDataAccess.GetCustomerRole(custIDs).ToDictionary(x => x.CustID, x => x.RoleID);

                robotInfo.RobotUsers = robotInfo.RobotUsers.Select(x =>
                {
                    dicCustRole.TryGetValue(x.CustId, out var roleID);

                    return new TWRobotUserEntity
                    {
                        CustId = x.CustId,
                        Counter = x.Counter,
                        RoleID = roleID
                    };
                });
            }

            return this;
        }

        public IRobotTWInsertServices GetRobotUserCategoryId(string serviceName)
        {
            if (robotInfo != null
                && robotInfo.RobotUsers != null
                && robotInfo.RobotUsers.Any())
            {
                robotCategoryId = GetCategoryByCategoryTypeName(Constants.CATEGORYTYPEPARENT_CATEID_ROBOTUSER);

                if (robotCategoryId == null)
                {
                    Utilities.LogSentryError(serviceName, "Get Invalid RobotUser CategoryID (null value)", robotCategoryId, null, _sentryClient);
                }
            }

            return this;
        }

        public bool InsertTWRobotPackage(int internalSize, string agencyApiUrl, PushDataToExternalEntity pushConfig)
        {           
            var isSuccess = true;

            if (robotInfo != null && robotInfo.RobotUsers != null && robotInfo.RobotUsers.Any())
            {                
                var agencyRobots = robotInfo.RobotUsers.Where(x => x.RoleID != 1).Select(x => x);

                isSuccess = InsertTWAgencyRobot(internalSize, agencyApiUrl, agencyRobots, pushConfig);

                if (isSuccess)
                {
                    var memberRobots = robotInfo.RobotUsers.Where(x => x.RoleID == 1).Select(x => x);

                    isSuccess = InsertTWMemberRobot(internalSize, memberRobots, pushConfig);
                }
            }

            return isSuccess;
        }

        private bool InsertTWMemberRobot(int internalSize, IEnumerable<TWRobotUserEntity> memberRobots, PushDataToExternalEntity pushConfig)
        {
            var isSuccess = true;

            var robotUsersChunks = memberRobots.Chunk(internalSize).ToList();

            foreach (var robotBatch in robotUsersChunks)
            {
                _dataLogTrackingServices.LogInputData(new LogInputViewEntity { RobotUser = robotBatch }, FunctionId.RobotUserJob, pushConfig.ServiceName);

                var insertedPA = robotBatch.Select(c => new InsertProblemAccountEntity(c, robotCategoryId));

                var custIds = InsertProblemAccountPackage(insertedPA, FunctionId.RobotUserJob, CCInputFlow.InsertPACategory, false, pushConfig.ServiceName);

                if (custIds != null && custIds.Any())
                {
                    isSuccess = _custClassToMainDBService.PushExternalPackageByCustId(custIds, pushConfig);
                }

                if (!isSuccess)
                {
                    return isSuccess;
                }

                Thread.Sleep(RobotUsers_SleepTimes);
            }

            return true;
        }

        private bool InsertTWAgencyRobot(int internalSize, string agencyApiUrl, IEnumerable<TWRobotUserEntity> agencyRobots, PushDataToExternalEntity pushConfig)
        {         
            _dataLogTrackingServices.LogInputData(new LogInputViewEntity { RobotUser = agencyRobots }, FunctionId.RobotUserJob, pushConfig.ServiceName);

            var robot = new InsertTWAgencyRobotModel { Robots = agencyRobots };

            var response = _ccAgencyService.CallAPIPost(robot, internalSize, pushConfig.ExternalBatchSize, FromFlow.CTSServices,
                agencyApiUrl + ApiEndpoint.InsertTWRobotAgencyEndpoint);

#pragma warning disable S4462 // Calls to "async" methods should not be blocking
            return response.Result;
#pragma warning restore S4462 // Calls to "async" methods should not be blocking
        }

        public void UpdateLastTimespan()
        {
            if (robotInfo != null
                && robotInfo.RobotUsers != null
                && robotInfo.RobotUsers.Any()
                && robotCategoryId != null)
            {
                string lastTimespan = robotInfo.MaxTimestamp.ToString();

                _systemParameterServices.UpdateTimingExecution(lastTimespan, CTS_SystemParameters.RobotAccountLastScannedTime);
            }
        }

        public TWRobotUserViewEntity RobotUserInfo => robotInfo;
    }
}