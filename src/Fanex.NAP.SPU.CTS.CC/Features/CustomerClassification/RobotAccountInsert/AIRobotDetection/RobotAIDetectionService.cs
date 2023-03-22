namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Fanex.NAP.SPU.CTS.Shares;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRobotAIDetectionService
    {
        IRobotAIDetectionService Start();

        IRobotAIDetectionService GetRobotDetections(SystemParameters robotAILastTime, SystemParameters robotImpervaLastTime);

        bool InsertRobotAIImpervaPackage(InsertRobotParaEntity paras);       

        IEnumerable<long> InsertRobotAIImperva(IEnumerable<RobotDetectionEntity> robotDetections, InsertRobotParaEntity paras);

        IEnumerable<InsertRobotEntity> MappingRobotCategory(IEnumerable<RobotDetectionEntity> robotDetections, InsertRobotParaEntity paras);

        IRobotAIDetectionService UpdateLastScannedId(SystemParameters robotAILastTime, SystemParameters robotImpervaLastTime);
    }

    public class RobotAIDetectionService : IRobotAIDetectionService
    {
        private readonly IRobotAIDetectionDataAccess dataAccess;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IProblemAccountServices problemAccountServices;
        private IEnumerable<RobotDetectionEntity> robotDetections;

        public RobotAIDetectionService(IRobotAIDetectionDataAccess dataAccess,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IProblemAccountServices problemAccountServices)
        {
            this.dataAccess = dataAccess;
            this.systemParameterServices = systemParameterServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.problemAccountServices = problemAccountServices;            
        }

        private string robotImpervaLogIDs;

        public IEnumerable<RobotDetectionEntity> RobotDetections => robotDetections;

        public IRobotAIDetectionService Start()
        {
            this.robotDetections = new List<RobotDetectionEntity>();
            this.robotImpervaLogIDs = string.Empty;

            return this;
        }

        public IRobotAIDetectionService GetRobotDetections(SystemParameters robotAILastTime, SystemParameters robotImpervaLastTime)
        {
            var lastTimeAI = systemParameterServices.GetLastScannedDateTime(robotAILastTime);

            var lastTimeImperva = systemParameterServices.GetLastScannedDateTime(robotImpervaLastTime);

            if (lastTimeAI != null || lastTimeImperva != null)
            {
                var (robotAI, robotImperva) = this.dataAccess.GetRobotDetections(lastTimeAI.Value, lastTimeImperva.Value);
                
                this.robotImpervaLogIDs = string.Join(",", robotImperva.Select(x => x.LogID));

                var robotImpervaDistinct = robotImperva.Distinct(x => x.CustID);

                this.robotDetections = CombineRobotAIImperva(robotAI, robotImpervaDistinct);
            }

            return this;
        }

        public bool InsertRobotAIImpervaPackage(InsertRobotParaEntity paras)
        {
            if (this.robotDetections != null && this.robotDetections.Any())
            {
                var robotDetectionChunks = this.robotDetections.Chunk(paras.BatchInternalSize);

                foreach (var robot in robotDetectionChunks)
                {
                    var custIds = InsertRobotAIImperva(robot, paras);

                    if (custIds != null && custIds.Any())
                    {
                        var isSuccess = custClassToMainDBService.PushDataToExternal(custIds, paras.PushDataExternal);

                        if (!isSuccess)
                        {
                            return false;
                        }

                        Thread.Sleep(RobotUsers_SleepTimes);
                    }
                }

                this.UpdateTransformRobotAIImperva();
            }

            return true;
        }

        public IRobotAIDetectionService UpdateLastScannedId(SystemParameters robotAILastTime, SystemParameters robotImpervaLastTime)
        {
            if (this.robotDetections != null && this.robotDetections.Any())
            {               
                var latestAITime = this.robotDetections.Max(x => x.LastModifiedDate);
                if (latestAITime != DateTime.MinValue)
                {
                    systemParameterServices.UpdateTimingExecution(latestAITime.ToString(CONVERT_TO_SQL_DATETIME), robotAILastTime);
                }

                var latestImpervaTime = this.robotDetections.Max(x => x.LastCreateTime);
                if (latestImpervaTime != DateTime.MinValue)
                {
                    systemParameterServices.UpdateTimingExecution(latestImpervaTime.ToString(CONVERT_TO_SQL_DATETIME), robotImpervaLastTime);
                }
            }

            return this;
        }

        public IEnumerable<long> InsertRobotAIImperva(IEnumerable<RobotDetectionEntity> robotDetections, InsertRobotParaEntity paras)
        {
            if (robotDetections != null && robotDetections.Any())
            {
                var robotToInsert = MappingRobotCategory(robotDetections, paras);

                var customerInfo = problemAccountServices.InsertRobotAccountPackage(robotToInsert, false);

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
                    IsFromAI = c.IsFromAI,
                    IsFromImperva = c.IsFromImperva,
                    IsFromTW = paras.IsFromTW,
                    IsFromCTS = paras.IsFromCTS,
                    IsDirect = paras.IsDirect,
                    IsFromTVS = paras.IsFromTVS,
                    RobotCounter = null,
                    CreatedBy = StarixITId
                }).ToList();
            }

            return custCategory;
        }

        private IEnumerable<RobotDetectionEntity> CombineRobotAIImperva(IEnumerable<RobotAIEntity> robotAI, IEnumerable<RobotImpervaEntity> robotImperva)
        {
            var result = robotAI.Select(x => new RobotDetectionEntity
            {
                CustID = x.CustID,
                LastModifiedDate = x.LastModifiedDate,
                LastCreateTime = DateTime.MinValue,
                IsFromAI = true,
                IsFromImperva = false
            }).Union(robotImperva.Select(x => new RobotDetectionEntity
            {
                CustID = x.CustID,
                LastModifiedDate = DateTime.MinValue,
                LastCreateTime = x.LastCreateTime,
                IsFromAI = false,
                IsFromImperva = true
            }).OrderBy(x => x.LastTimeOrderBy));

            return result;
        }

        private void UpdateTransformRobotAIImperva()
        {
            if (!string.IsNullOrEmpty(this.robotImpervaLogIDs))
            {
                dataAccess.UpdateTransformRobotAIImperva(this.robotImpervaLogIDs);
            }
        }
    }
}