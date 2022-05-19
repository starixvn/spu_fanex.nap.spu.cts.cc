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

    public interface IRobotAIClassificationInsertServices
    {
        IRobotAIClassificationInsertServices Start();

        IRobotAIClassificationInsertServices GetRobotUserFromAIClassification();

        bool Process(InsertRobotParaEntity paras);

        IRobotAIClassificationInsertServices UpdateLastScannedId(SystemParameters index);

        bool InsertPAToCTS(IEnumerable<RobotAIEntity> robotUsers, InsertRobotParaEntity paras);

        IEnumerable<RobotInsetViewEntity> MappingRobotCategory(IEnumerable<RobotAIEntity> robotUsers);
    }

    public class RobotAIClassificationInsertServices : IRobotAIClassificationInsertServices
    {
        private readonly IRobotAIClassificationInsertDataAccess robotAIClassificationInsertDataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private RobotAIViewEntity robotAIInfo;

        public RobotAIClassificationInsertServices(
            IRobotAIClassificationInsertDataAccess robotAIClassificationInsertDataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices)

        {
            this.robotAIClassificationInsertDataAccess = robotAIClassificationInsertDataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;
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
                    bool isSuccess = InsertPAToCTS(robot, paras);

                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool InsertPAToCTS(IEnumerable<RobotAIEntity> robotUsers, InsertRobotParaEntity paras)
        {
            bool isSuccess = true;

            if (robotUsers != null && robotUsers.Any())
            {
                var custCategory = MappingRobotCategory(robotUsers);

                //var result = problemAccountServices.InsertProblemAccount(custCategory, false, paras.ServiceName);

                //isSuccess = (result != null && result.ErrorCode == 0);

                //if (isSuccess && result.CustIds != null && result.CustIds.Any())
                //{
                //    isSuccess = custClassToMainDBService.PushDataToExternal(result.CustIds, new PushDataToExternalEntity
                //    {
                //        ExternalBatchSize = paras.BatchExternalSize,
                //        FeedSoon88API = paras.Soon88FeedDataAPI,
                //        IsPushCCToSoon88 = true,
                //        IsUpdateDangerToMainDB = true,
                //        DangerLevelType = DangerLevel.ABI,
                //        IsWriteLog = paras.IsWriteLog,
                //        ServiceName = paras.ServiceName
                //    });

                //    Thread.Sleep(RobotUsers_SleepTimes);
                //}
            }

            return isSuccess;
        }

        public IEnumerable<RobotInsetViewEntity> MappingRobotCategory(IEnumerable<RobotAIEntity> robotUsers)
        {
            var custCategory = new List<RobotInsetViewEntity>();

            if (robotUsers != null && robotUsers.Any())
            {
                custCategory = robotUsers.Select(c => new RobotInsetViewEntity
                {
                    CustID = c.CustID,
                    ReasonID = c.ReasonID,
                    RobotCounter = 0,
                    Creator= StarixITId
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