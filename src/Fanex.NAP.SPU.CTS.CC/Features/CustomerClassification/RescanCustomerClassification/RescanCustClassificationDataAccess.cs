namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using System.Collections.Generic;

    public interface IRescanCustClassificationDataAccess
    {
        IEnumerable<RoleCustomerEntity> GetRoleCustomer(string custIds);

        IEnumerable<TWRobotUserEntity> GetExistedTWRobotUser(string custIds);
    }

    public class RescanCustClassificationDataAccess : IRescanCustClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public RescanCustClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RoleCustomerEntity> GetRoleCustomer(string custIds)
        {
            var criteria = new RoleCustomerGetCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<RoleCustomerEntity>(criteria);
        }

        public IEnumerable<TWRobotUserEntity> GetExistedTWRobotUser(string custIds)
        {
            var criteria = new ExistedTWRobotUserGetCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<TWRobotUserEntity>(criteria);
        }
    }
}