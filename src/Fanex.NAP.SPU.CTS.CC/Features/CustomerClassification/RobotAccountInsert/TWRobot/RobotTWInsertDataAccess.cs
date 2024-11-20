namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using System.Collections.Generic;

    public interface IRobotTWInsertDataAccess
    {
        TWRobotUserViewEntity GetTWRobotUsers(long lastTimestamp);

        IEnumerable<RoleCustomerEntity> GetCustomerRole(string custIds);
    }

    public class RobotTWInsertDataAccess : BaseCustomerClassificationDataAccess, IRobotTWInsertDataAccess
    {
        private readonly IDynamicRepository repository;

        public RobotTWInsertDataAccess()
        {
            repository = new DynamicRepository();
        }        

        public TWRobotUserViewEntity GetTWRobotUsers(long lastTimestamp)
        {
            var result = new TWRobotUserViewEntity
            {
                RobotUsers = new List<TWRobotUserEntity>(),
                MaxTimestamp = 0
            };

            var criteria = new RobotUserGetCriteria
            {
                LastTimestamp = lastTimestamp
            };

            result.RobotUsers = repository.Fetch<TWRobotUserEntity>(criteria);
            result.MaxTimestamp = criteria.MaxTimestamp;

            return result;
        }

        public IEnumerable<RoleCustomerEntity> GetCustomerRole(string custIds)
        {
            var criteria = new RoleCustomerGetCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<RoleCustomerEntity>(criteria);
        }
    }
}