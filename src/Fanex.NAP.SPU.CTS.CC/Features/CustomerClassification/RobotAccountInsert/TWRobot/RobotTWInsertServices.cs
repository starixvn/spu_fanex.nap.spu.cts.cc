namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
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

    public interface IRobotTWInsertServices
    {
        IRobotTWInsertServices Start();

        IRobotTWInsertServices GetLastTimespan();

        IRobotTWInsertServices GetTWRobotUser();

        bool InsertTWRobotPackage(InsertRobotParaEntity paras);

        IEnumerable<long> InsertTWRobot(IEnumerable<TWRobotUserEntity> robotUsers, InsertRobotParaEntity paras);

        IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<TWRobotUserEntity> robotUsers, InsertRobotParaEntity paras);

        void UpdateLastTimespan();

        TWRobotUserViewEntity RobotUserInfo { get; }
    }

    public class RobotTWInsertServices : IRobotTWInsertServices
    {
        private readonly IRobotTWInsertDataAccess robotAccountInsertDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IDataLogTrackingServices dataLogTrackingServices;
        private long lastScannedTimespan;
        private TWRobotUserViewEntity robotInfo;

        public RobotTWInsertServices(
            IRobotTWInsertDataAccess robotAccountInsertDataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices,
            IDataLogTrackingServices dataLogTrackingServices)

        {
            this.robotAccountInsertDataAccess = robotAccountInsertDataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;
            this.dataLogTrackingServices = dataLogTrackingServices;
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
            lastScannedTimespan = systemParameterServices.GetLastScannedId(CTS_DC_SystemParameters.RobotAccountLastScannedTime);

            return this;
        }

        public IRobotTWInsertServices GetTWRobotUser()
        {
            robotInfo = robotAccountInsertDataAccess.GetTWRobotUsers(lastScannedTimespan);

            return this;
        }

        public bool InsertTWRobotPackage(InsertRobotParaEntity paras)
        {
            if (robotInfo != null && robotInfo.RobotUsers != null && robotInfo.RobotUsers.Any())
            {
                var robotUsersChunks = robotInfo.RobotUsers.Chunk(paras.BatchInternalSize).ToList();

                foreach (var robotBatch in robotUsersChunks)
                {
                    var custIds = InsertTWRobot(robotBatch, paras);

                    if (custIds != null && custIds.Any())
                    {
                        var isSuccess = custClassToMainDBService.PushDataToExternal(custIds, paras.PushDataExternal);

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

        public IEnumerable<long> InsertTWRobot(IEnumerable<TWRobotUserEntity> robotUsers, InsertRobotParaEntity paras)
        {
            if (robotUsers != null && robotUsers.Any())
            {
                var robotToInsert = MappingRobotCategory(robotUsers, paras);

                dataLogTrackingServices.LogInputData(new LogInputViewEntity { RobotUser = robotToInsert }, FunctionId.RobotUserJob, paras.PushDataExternal.ServiceName);

                var customerInfo = problemAccountServices.InsertRobotAccountPackage(robotToInsert, false);

                if (customerInfo != null && customerInfo.Any())
                {
                    return customerInfo.Select(x => x.CustID).Distinct();
                }
            }

            return Enumerable.Empty<long>();
        }

        public IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<TWRobotUserEntity> robotUsers, InsertRobotParaEntity paras)
        {
            var custCategory = new List<InsertRobotEntity>();

            if (robotUsers != null && robotUsers.Any())
            {
                custCategory = robotUsers.Select(c => new InsertRobotEntity
                {
                    CustID = c.CustId,
                    IsFromTW = paras.IsFromTW,
                    IsFromCTS = paras.IsFromCTS,
                    IsDirect = paras.IsDirect,
                    IsFromAI = paras.IsFromAI,
                    IsFromTVS = paras.IsFromTVS,
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

                systemParameterServices.UpdateTimingExecution(lastTimespan, CTS_DC_SystemParameters.RobotAccountLastScannedTime);
            }
        }

        public TWRobotUserViewEntity RobotUserInfo => robotInfo;
    }
}