namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAIClassificationInsertServices
    {
        IRobotAIClassificationInsertServices Start();

        IRobotAIClassificationInsertServices GetRobotUserFromAIClassification();

        bool Process(InsertRobotParaEntity paras);

        IRobotAIClassificationInsertServices UpdateLastScannedId(SystemParameters index);

        IEnumerable<long> InsertPAToCTS(IEnumerable<RobotAIEntity> robotUsers, InsertRobotParaEntity paras);

        IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<RobotAIEntity> robotUsers, InsertRobotParaEntity paras);
    }

    public class RobotAIClassificationInsertServices : IRobotAIClassificationInsertServices
    {
        private readonly IRobotAIClassificationInsertDataAccess robotAIClassificationInsertDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private RobotAIViewEntity robotAIInfo;

        public RobotAIClassificationInsertServices(
            IRobotAIClassificationInsertDataAccess robotAIClassificationInsertDataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices,
            IAntiFraudCenterServices antiFraudCenterServices)

        {
            this.robotAIClassificationInsertDataAccess = robotAIClassificationInsertDataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;
            this.antiFraudCenterServices = antiFraudCenterServices;
        }

        public IRobotAIClassificationInsertServices Start()
        {
            robotAIInfo = new RobotAIViewEntity
            {
                LastScannedId = 0,
                RobotAIClassification = Enumerable.Empty<RobotAIEntity>()
            };

            return this;
        }

        public IRobotAIClassificationInsertServices GetRobotUserFromAIClassification()
        {
            robotAIInfo = robotAIClassificationInsertDataAccess.GetRobotUsersFromAIClassification();

            return this;
        }

        public bool Process(InsertRobotParaEntity paras)
        {
            if (robotAIInfo != null && robotAIInfo.RobotAIClassification != null && robotAIInfo.RobotAIClassification.Any())
            {
                var robotUsersChunks = robotAIInfo.RobotAIClassification.Chunk(paras.BatchInternalSize).ToList();

                foreach (var robot in robotUsersChunks)
                {
                    var custIds = InsertPAToCTS(robot, paras);

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

                        Thread.Sleep(RobotUsers_SleepTimes);
                    }                   
                }
            }

            return true;
        }

        public IEnumerable<long> InsertPAToCTS(IEnumerable<RobotAIEntity> robotUsers, InsertRobotParaEntity paras)
        {          
            if (robotUsers != null && robotUsers.Any())
            {
                var robotToInsert = MappingRobotCategory(robotUsers, paras);

                var customerInfo = problemAccountServices.InsertRobotAccount(robotToInsert);

                if (customerInfo != null && customerInfo.Any())
                {
                    return customerInfo.Select(x => x.CustID).Distinct();
                }
            }

            return Enumerable.Empty<long>();
        }

        public IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<RobotAIEntity> robotUsers, InsertRobotParaEntity paras)
        {
            var custCategory = new List<InsertRobotEntity>();

            if (robotUsers != null && robotUsers.Any())
            {
                custCategory = robotUsers.Select(c => new InsertRobotEntity
                {
                    CustID = c.CustID,
                    CategoryID = (int)Category.AIRobotClassification,
                    IsFromTW = paras.IsFromTW,
                    IsFromCTS = paras.IsFromCTS,
                    IsDirect = paras.IsDirect,
                    IsFromAI = paras.IsFromAI,
                    RobotCounter = 0,
                    CreatedBy = StarixITId
                }).ToList();
            }

            return custCategory;
        }

        public IRobotAIClassificationInsertServices UpdateLastScannedId(SystemParameters index)
        {
            if (robotAIInfo != null && robotAIInfo.LastScannedId != 0)
            {
                string lastScannedId = robotAIInfo.LastScannedId.ToString();

                systemParameterServices.UpdateTimingExecution(lastScannedId, index);
            }

            return this;
        }

        public RobotAIViewEntity RobotUserInfo => robotAIInfo;
    }
}