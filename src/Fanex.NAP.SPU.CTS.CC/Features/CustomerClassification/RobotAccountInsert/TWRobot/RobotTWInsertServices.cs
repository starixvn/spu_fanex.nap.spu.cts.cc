namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotTWInsertServices
    {
        IRobotTWInsertServices Start();

        IRobotTWInsertServices GetLastTimespan();

        IRobotTWInsertServices GetTWRobotUser();

        IRobotTWInsertServices GetRobotUserCategoryId(string serviceName);

        bool InsertTWRobotPackage(int internalSize, PushDataToExternalEntity pushConfig);

        void UpdateLastTimespan();

        TWRobotUserViewEntity RobotUserInfo { get; }
    }

    public class RobotTWInsertServices : ProblemAccountServices, IRobotTWInsertServices
    {
        private readonly IRobotTWInsertDataAccess robotAccountInsertDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IDataLogTrackingServices dataLogTrackingServices;
        private readonly ISentryClient sentryClient;
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
            ISentryClient sentryClient) : base(baseDataAccess, problemAccountDataAccess, sentryClient)

        {
            this.robotAccountInsertDataAccess = robotAccountInsertDataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.dataLogTrackingServices = dataLogTrackingServices;
            this.sentryClient = sentryClient;
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
            lastScannedTimespan = systemParameterServices.GetLastScannedId(CTS_SystemParameters.RobotAccountLastScannedTime);

            return this;
        }

        public IRobotTWInsertServices GetTWRobotUser()
        {
            robotInfo = robotAccountInsertDataAccess.GetTWRobotUsers(lastScannedTimespan);

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
                    Utilities.LogSentryError(serviceName, "Get Invalid RobotUser CategoryID (null value)", robotCategoryId, null, sentryClient);
                }
            }

            return this;
        }

        public bool InsertTWRobotPackage(int internalSize, PushDataToExternalEntity pushConfig)
        {
            if (robotInfo != null 
                && robotInfo.RobotUsers != null 
                && robotInfo.RobotUsers.Any()
                && robotCategoryId != null)
            {
                var robotUsersChunks = robotInfo.RobotUsers.Chunk(internalSize).ToList();

                foreach (var robotBatch in robotUsersChunks)
                {
                    dataLogTrackingServices.LogInputData(new LogInputViewEntity { RobotUser = robotBatch }, FunctionId.RobotUserJob, pushConfig.ServiceName);

                    var insertedPA = robotBatch.ToList().Select(c => new InsertProblemAccountEntity(c, robotCategoryId));

                    var custIds = InsertProblemAccountPackage(insertedPA, FunctionId.RobotUserJob, CCInputFlow.InsertPACategory, false, pushConfig.ServiceName);

                    if (custIds != null && custIds.Any())
                    {
                        var isSuccess = custClassToMainDBService.PushExternalPackageByCustId(custIds, pushConfig);

                        if (!isSuccess)
                        {
                            return false;
                        }
                    }

                    Thread.Sleep(RobotUsers_SleepTimes);
                }
            }

            return true;
        }

        public void UpdateLastTimespan()
        {
            if (robotInfo != null
                && robotInfo.RobotUsers != null
                && robotInfo.RobotUsers.Any()
                && robotCategoryId != null)
            {
                string lastTimespan = robotInfo.MaxTimestamp.ToString();

                systemParameterServices.UpdateTimingExecution(lastTimespan, CTS_SystemParameters.RobotAccountLastScannedTime);
            }
        }

        public TWRobotUserViewEntity RobotUserInfo => robotInfo;
    }
}