namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface IRobotAIClassificationInsertDataAccess
    {
        RobotAIViewEntity GetRobotUsersFromAIClassification();
    }

    public class RobotAIClassificationInsertDataAccess : IRobotAIClassificationInsertDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotAIClassificationInsertDataAccess()
        {
            repository = new DynamicRepository();
        }

        public RobotAIViewEntity GetRobotUsersFromAIClassification()
        {
            var criteria = new RobotAIGetCriteria();

            var robotAIClassification = repository.Fetch<RobotAIEntity>(criteria);

            return new RobotAIViewEntity
            {
                RobotAIClassification = robotAIClassification ?? new List<RobotAIEntity>(),
                LastScannedId = criteria.LastID
            };
        }
    }
}