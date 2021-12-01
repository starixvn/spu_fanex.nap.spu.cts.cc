namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAccountInsertServices
    {
        IRobotAccountInsertServices Start();

        IRobotAccountInsertServices GetLastTimespan();

        IRobotAccountInsertServices GetRobotUser();

        bool Process(InsertRobotParaEntity paras);

        bool InsertRobotUsers(IEnumerable<RobotUserEntity> robotUsers, InsertRobotParaEntity paras);

        IEnumerable<ProblemAccountsEntity> MappingRobotCategory(IEnumerable<RobotUserEntity> robotUsers);

        void UpdateLastTimespan();

        RobotUserViewEntity RobotUserInfo { get; }
    }

    public class RobotAccountInsertServices : IRobotAccountInsertServices
    {
        private readonly IRobotAccountInsertDataAccess robotAccountInsertDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private long lastScannedTimespan;
        private RobotUserViewEntity robotInfo;

        public RobotAccountInsertServices(
            IRobotAccountInsertDataAccess robotAccountInsertDataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices)

        {
            this.robotAccountInsertDataAccess = robotAccountInsertDataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;
        }

        public IRobotAccountInsertServices Start()
        {
            lastScannedTimespan = 0;

            robotInfo = new RobotUserViewEntity
            {
                RobotUsers = new List<RobotUserEntity>(),
                MaxTimestamp = 0
            };

            return this;
        }

        public IRobotAccountInsertServices GetLastTimespan()
        {
            lastScannedTimespan = systemParameterServices.GetLastScannedId(SystemParameters.RobotAccountLastScannedTime);

            return this;
        }

        public IRobotAccountInsertServices GetRobotUser()
        {
            robotInfo = robotAccountInsertDataAccess.GetRobotUsers(lastScannedTimespan);

            return this;
        }

        public bool Process(InsertRobotParaEntity paras)
        {
            if (robotInfo != null
                && robotInfo.RobotUsers != null
                && robotInfo.RobotUsers.Any())
            {
                custClassToMainDBService.LogInputData(new LogInputViewEntity { RobotUser = robotInfo.RobotUsers }, FunctionId.RobotUserJob, paras.ServiceName);

                var robotUsersChunks = robotInfo.RobotUsers.Chunk(paras.BatchInternalSize).ToList();

                foreach (var robot in robotUsersChunks)
                {
                    bool isSuccess = InsertRobotUsers(robot, paras);

                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool InsertRobotUsers(IEnumerable<RobotUserEntity> robotUsers, InsertRobotParaEntity paras)
        {
            bool isSuccess = true;

            if (robotUsers != null && robotUsers.Any())
            {
                var custCategory = MappingRobotCategory(robotUsers);

                var result = problemAccountServices.InsertProblemAccountToCTS(custCategory, false, paras.ServiceName, paras.IsWriteLog);
               
                isSuccess = (result != null && result.ErrorCode == 0);

                if (isSuccess && result.CustIds != null && result.CustIds.Any())
                {
                    isSuccess = custClassToMainDBService.PushDataToExternal(result.CustIds, new PushDataToExternalEntity
                    {
                        ExternalBatchSize = paras.BatchExternalSize,
                        FeedSoon88API = paras.Soon88FeedDataAPI,
                        IsPushCCToSoon88 = true,
                        IsUpdateDangerToMainDB = true,
                        DangerLevelType = DangerLevel.ABI,
                        IsWriteLog = paras.IsWriteLog,
                        ServiceName = paras.ServiceName
                    });

                    Thread.Sleep(RobotUsers_SleepTimes);
                }
            }

            return isSuccess;
        }

        public IEnumerable<ProblemAccountsEntity> MappingRobotCategory(IEnumerable<RobotUserEntity> robotUsers)
        {
            var custCategory = new List<ProblemAccountsEntity>();

            if (robotUsers != null && robotUsers.Any())
            {
                custCategory = robotUsers.Select(c => new ProblemAccountsEntity
                {
                    CustID = c.CustId,
                    ReasonID = (int)Category.Robot,
                    RobotCounter = c.Counter
                }).ToList();
            }

            return custCategory;
        }

        public void UpdateLastTimespan()
        {
            if (robotInfo != null
                && robotInfo.RobotUsers != null
                && robotInfo.RobotUsers.Any())
            {
                string lastTimespan = robotInfo.MaxTimestamp.ToString();

                systemParameterServices.UpdateTimingExecution(lastTimespan, SystemParameters.RobotAccountLastScannedTime);
            }
        }

        public RobotUserViewEntity RobotUserInfo => robotInfo;
    }
}