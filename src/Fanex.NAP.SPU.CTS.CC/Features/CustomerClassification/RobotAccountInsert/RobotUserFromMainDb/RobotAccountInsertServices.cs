namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
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

        IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<RobotUserEntity> robotUsers, InsertRobotParaEntity paras);

        void UpdateLastTimespan();

        RobotUserViewEntity RobotUserInfo { get; }
    }

    public class RobotAccountInsertServices : IRobotAccountInsertServices
    {
        private readonly IRobotAccountInsertDataAccess robotAccountInsertDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly IDataLogTrackingServices dataLogTrackingServices;
        private long lastScannedTimespan;
        private RobotUserViewEntity robotInfo;

        public RobotAccountInsertServices(
            IRobotAccountInsertDataAccess robotAccountInsertDataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices,
            IAntiFraudCenterServices antiFraudCenterServices,
            IDataLogTrackingServices dataLogTrackingServices)

        {
            this.robotAccountInsertDataAccess = robotAccountInsertDataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.dataLogTrackingServices = dataLogTrackingServices;
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
            robotInfo = robotAccountInsertDataAccess.GetRobotUsersFromMainDb(lastScannedTimespan);

            return this;
        }

        public bool Process(InsertRobotParaEntity paras)
        {
            if (robotInfo != null
                && robotInfo.RobotUsers != null
                && robotInfo.RobotUsers.Any())
            {
                var robotUsersChunks = robotInfo.RobotUsers.Chunk(paras.BatchInternalSize).ToList();

                foreach (var robot in robotUsersChunks)
                {
                    var custIds = InsertRobotUsers(robot, paras);

                    if (custIds != null && custIds.Any())
                    {
                        var isSuccess = custClassToMainDBService.PushDataToExternal(custIds, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = paras.BatchExternalSize,
                            FeedSoon88API = paras.Soon88FeedDataAPI,
                            IsPushCCToSoon88 = paras.IsPushCCToSoon88,
                            IsUpdateDangerToMainDB = true,
                            IsWriteLog = paras.IsWriteLog,
                            ServiceName = paras.ServiceName
                        });

                        antiFraudCenterServices.InsertAFCAccounts(custIds, FromFlow.CTSServices, paras.IsSyncAFCData, paras.AFCInsertAccountURL);

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

        public IEnumerable<long> InsertRobotUsers(IEnumerable<RobotUserEntity> robotUsers, InsertRobotParaEntity paras)
        {
            if (robotUsers != null && robotUsers.Any())
            {
                var robotToInsert = MappingRobotCategory(robotUsers, paras);

                dataLogTrackingServices.LogInputData(new LogInputViewEntity { RobotUser = robotToInsert }, FunctionId.RobotUserJob, paras.ServiceName);

                var customerInfo = problemAccountServices.InsertRobotAccount(robotToInsert);

                if (customerInfo != null && customerInfo.Any())
                {
                    return customerInfo.Select(x => x.CustID).Distinct();
                }
            }

            return Enumerable.Empty<long>();
        }

        public IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<RobotUserEntity> robotUsers, InsertRobotParaEntity paras)
        {
            var custCategory = new List<InsertRobotEntity>();

            if (robotUsers != null && robotUsers.Any())
            {
                custCategory = robotUsers.Select(c => new InsertRobotEntity
                {
                    CustID = c.CustId,
                    CategoryID = (int)Category.Robot,
                    IsFromTW = paras.IsFromTW,
                    IsFromCTS = paras.IsFromCTS,
                    IsDirect = paras.IsDirect,
                    IsFromAI = paras.IsFromAI,
                    RobotCounter = c.Counter,
                    CreatedBy = StarixITId
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