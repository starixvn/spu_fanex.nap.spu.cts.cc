namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface IRobotAccountInsertDataAccess
    {
        RobotUserViewEntity GetRobotUsersFromMainDb(long lastTimestamp);
    }

    public class RobotAccountInsertDataAccess : IRobotAccountInsertDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotAccountInsertDataAccess()
        {
            repository = new DynamicRepository();
        }

        public RobotUserViewEntity GetRobotUsersFromMainDb(long lastTimestamp)
        {
            var result = new RobotUserViewEntity
            {
                RobotUsers = new List<RobotUserEntity>(),
                MaxTimestamp = 0
            };

            var criteria = new RobotUserGetCriteria
            {
                LastTimestamp = lastTimestamp
            };

            result.RobotUsers = repository.Fetch<RobotUserEntity>(criteria);
            result.MaxTimestamp = criteria.MaxTimestamp;

            return result;
        }
    }
}