namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotDetectionService
    {
        IRobotDetectionService Start();

        IRobotDetectionService GetRobotDetections(SystemParameters robotDetectionLastModifiedDate);

        bool Process(InsertRobotParaEntity paras);

        IEnumerable<long> InsertProblemAccount(IEnumerable<RobotDetectionEntity> robotDetections, InsertRobotParaEntity paras);

        IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<RobotDetectionEntity> robotDetections, InsertRobotParaEntity paras);

        IRobotDetectionService UpdateLastScannedId(SystemParameters index);
    }

    public class RobotDetectionService : IRobotDetectionService
    {
        private readonly IRobotDetectionDataAccess dataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private IEnumerable<RobotDetectionEntity> robotDetections;

        public RobotDetectionService(IRobotDetectionDataAccess dataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices,
            IAntiFraudCenterServices antiFraudCenterServices)
        {
            this.dataAccess = dataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;
            this.antiFraudCenterServices = antiFraudCenterServices;            
        }

        public IEnumerable<RobotDetectionEntity> RobotDetections => robotDetections;

        public IRobotDetectionService Start()
        {
            this.robotDetections = new List<RobotDetectionEntity>();

            return this;
        }

        public IRobotDetectionService GetRobotDetections(SystemParameters robotDetectionLastModifiedDate)
        {            
            var lastScannedTime = systemParameterServices.GetLastScannedDateTime(robotDetectionLastModifiedDate);

            if (lastScannedTime != null)
            {
                this.robotDetections = this.dataAccess.GetRobotDetections(lastScannedTime.Value);
            }            

            return this;
        }

        public bool Process(InsertRobotParaEntity paras)
        {
            if (this.robotDetections != null && this.robotDetections.Any())
            {
                var robotDetectionChunks = this.robotDetections.Chunk(paras.BatchInternalSize);

                foreach (var robot in robotDetectionChunks)
                {
                    var custIds = InsertProblemAccount(robot, paras);

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

        public IRobotDetectionService UpdateLastScannedId(SystemParameters index)
        {
            if (this.robotDetections != null && this.robotDetections.Any())
            {
                var latestScannedTime = this.robotDetections.FirstOrDefault().LastModifiedDate.ToString();

                systemParameterServices.UpdateTimingExecution(latestScannedTime, index);
            }

            return this;
        }

        public IEnumerable<long> InsertProblemAccount(IEnumerable<RobotDetectionEntity> robotDetections, InsertRobotParaEntity paras)
        {
            if (robotDetections != null && robotDetections.Any())
            {
                var robotToInsert = MappingRobotCategory(robotDetections, paras);

                var customerInfo = problemAccountServices.InsertRobotAccount(robotToInsert);

                if (customerInfo != null && customerInfo.Any())
                {
                    return customerInfo.Select(x => x.CustID).Distinct();
                }
            }

            return Enumerable.Empty<long>();
        }

        public IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<RobotDetectionEntity> robotDetections, InsertRobotParaEntity paras)
        {
            var custCategory = new List<InsertRobotEntity>();

            if (robotDetections != null && robotDetections.Any())
            {
                custCategory = robotDetections.Select(c => new InsertRobotEntity
                {
                    CustID = c.CustID,
                    CategoryID = (int)Category.AIRobotDetection,
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

    }
}
