namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.AIRobotDetection.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using System;
    using System.Collections.Generic;

    public interface IRobotAIDetectionDataAccess
    {
        (IEnumerable<RobotAIEntity> robotAI, IEnumerable<RobotImpervaEntity> robotImperva) GetRobotDetections(DateTime latestScannedTime, DateTime lastRobotImpervaTime);

        void UpdateTransformRobotAIImperva(string logIDs);
    }

    public class RobotAIDetectionDataAccess : IRobotAIDetectionDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotAIDetectionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public (IEnumerable<RobotAIEntity> robotAI, IEnumerable<RobotImpervaEntity> robotImperva) GetRobotDetections(DateTime latestTimeAI, DateTime latestTimeImperva)
        {              
            return repository.FetchMultiple<RobotAIEntity, RobotImpervaEntity>(new RobotDetectionGetLatestCriteria
            {
                LatestTimeAI = latestTimeAI,
                LatestTimeImperva = latestTimeImperva
            });
        }

        public void UpdateTransformRobotAIImperva(string logIDs)
        {
            var criteria = new RobotImpervaUpdateTransformCriteria { LogIDs = logIDs };

            repository.Execute(criteria);
        }
    }
}