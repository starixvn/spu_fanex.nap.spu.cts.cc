namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanCustClassificationDataAccess
    {
        IEnumerable<RoleCustomerEntity> GetRoleCustomer(string custIds);

        IEnumerable<TWRobotUserEntity> GetExistedTWRobotUser(string custIds);

        void RescanNormalClassification(string custList, FunctionId functionId, RescanNormalType type);
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

        public void RescanNormalClassification(string custList, FunctionId functionId, RescanNormalType type)
        {
            var criteria = new NormalClassificationRescanCriteria
            {
                ListCustId = custList,
                FunctionId = (int)functionId,
                RescanTypeId = (int)type

            };

            repository.Execute(criteria);
        }
    }
}