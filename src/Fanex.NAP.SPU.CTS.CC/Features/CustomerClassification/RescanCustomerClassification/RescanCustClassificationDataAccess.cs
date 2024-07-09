namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanCustClassificationDataAccess
    {
        IEnumerable<RoleCustomerEntity> GetRoleCustomer(string custIds);

        IEnumerable<TWRobotUserEntity> GetExistedTWRobotUser(string custIds);
        
        int PreNormalClassification(string custList, FunctionId functionId, RescanNormalType type);
       
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

        public int PreNormalClassification(string custList, FunctionId functionId, RescanNormalType type)
        {
            var criteria = new NormalClassificationRescanCriteria
            {
                ListCustId = custList,
                FunctionId = (int)functionId,
                RescanTypeId = (int)type

            };

            return repository.Execute(criteria);
        }

    }
}