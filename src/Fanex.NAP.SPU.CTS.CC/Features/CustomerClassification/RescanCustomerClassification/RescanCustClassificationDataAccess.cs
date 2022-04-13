namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;    
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRescanCustClassificationDataAccess
    {
        IEnumerable<RoleCustomerEntity> GetRoleCustomer(string custIds);

        IEnumerable<CustEvidenceEntity> GetExistedEvidences(string custIDs);

        IEnumerable<RobotUserEntity> GetExistedRobotUser(string custIds);
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

        public IEnumerable<CustEvidenceEntity> GetExistedEvidences(string custIDs)
        {
            var criteria = new ExistedEvidencesGetCriteria
            {
                CustIds = custIDs
            };

            return repository.Fetch<CustEvidenceEntity>(criteria);
        }

        public IEnumerable<RobotUserEntity> GetExistedRobotUser(string custIds)
        {
            var criteria = new ExistedRobotUserGetCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<RobotUserEntity>(criteria);
        }
    }
}