namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert.Entities;
    using System.Collections.Generic;

    public interface IRobotAIDetectionDataAccess
    {
        (IEnumerable<RobotAIEntity> robotAI, IEnumerable<RobotImpervaEntity> robotImperva) GetRobotDetections(long lastRobotAIID, long lastRobotImpervaID);

    }

    public class RobotAIDetectionDataAccess : BaseCustomerClassificationDataAccess, IRobotAIDetectionDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotAIDetectionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public (IEnumerable<RobotAIEntity> robotAI, IEnumerable<RobotImpervaEntity> robotImperva) GetRobotDetections(long lastRobotAIID, long lastRobotImpervaID)
        {
            return repository.FetchMultiple<RobotAIEntity, RobotImpervaEntity>(new RobotDetectionGetLatestCriteria
            {
                LastRobotAIID = lastRobotAIID,
                LastRobotImpervaID = lastRobotImpervaID
            });
        }
       
    }
}