namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using System;
    using System.Collections.Generic;

    public interface IRobotDetectionDataAccess
    {
        IEnumerable<RobotDetectionEntity> GetRobotDetections(DateTime latestScannedTime);
    }

    public class RobotDetectionDataAccess : IRobotDetectionDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotDetectionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RobotDetectionEntity> GetRobotDetections(DateTime latestScannedTime)
        {
            var criteria = new RobotDetectionGetLatestCriteria
            {
                LatestScannedTime = latestScannedTime
            };

            return repository.Fetch<RobotDetectionEntity>(criteria);
        }

    }
}
