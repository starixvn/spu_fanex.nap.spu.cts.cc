namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using System.Collections.Generic;

    public interface IRobotAIDetectionDataAccess
    {
        IEnumerable<RobotAIEntity> GetRobotDetections(long lastRobotAIID);
    }

    public class RobotAIDetectionDataAccess : BaseCustomerClassificationDataAccess, IRobotAIDetectionDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotAIDetectionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RobotAIEntity> GetRobotDetections(long lastRobotAIID)
        {
            return repository.Fetch<RobotAIEntity>(new RobotDetectionGetLatestCriteria
            {
                LastRobotAIID = lastRobotAIID
            });
        }
    }
}