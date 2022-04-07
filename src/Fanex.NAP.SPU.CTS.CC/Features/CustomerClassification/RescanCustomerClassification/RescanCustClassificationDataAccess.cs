namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;    
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRescanCustClassificationDataAccess
    {
        IEnumerable<RoleCustomerEntity> GetRoleCustomer(string custIds);

        IEnumerable<CustEvidenceEntity> GetExistedEvidences(string custIDs);

        IEnumerable<RobotUserEntity> GetExistedRobotUser(string custIds);

        DBResult SyncCustomerEvidenceToCategory(string jsonData, bool isRescan);
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

        public DBResult SyncCustomerEvidenceToCategory(string jsonData, bool isRescan)
        {
            var custInfos = new List<CheckPACustomerAssociationForRollbackEntity>();

            var criteria = new SyncCustEvidenceToCTSCategoryCriteria
            {
                CustEvidences = jsonData,
                IsRescan = isRescan
            };

            if (!string.IsNullOrEmpty(jsonData))
            {
                custInfos = repository.Fetch<CheckPACustomerAssociationForRollbackEntity>(criteria).ToList();
            }

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage,
                OutParam = new Hashtable {
                    { "custInfos", custInfos }
                }
            };
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