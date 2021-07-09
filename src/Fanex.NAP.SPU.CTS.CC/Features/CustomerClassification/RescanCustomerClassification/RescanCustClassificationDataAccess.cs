namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRescanCustClassificationDataAccess
    {
        IEnumerable<RoleCustomerEntity> GetRoleCustomer(string custIds);

        IEnumerable<CustEvidenceEntity> GetExistedEvidences(string custIDs);

        string GetExistedRobotUser(string custIds);

        IEnumerable<int> GetCustByRobotSiteRule(string custIds);

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
            var custIds = new List<long>();

            var criteria = new SyncCustEvidenceToCTSCategoryCriteria
            {
                CustEvidences = jsonData,
                IsRescan = isRescan
            };

            if (!string.IsNullOrEmpty(jsonData))
            {
                custIds = repository.Fetch<long>(criteria).ToList();
            }

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : -1,
                ErrorMsg = criteria.ErrorMessage,
                OutParam = new Hashtable {
                    { "custIds", custIds }
                }
            };
        }

        public IEnumerable<int> GetCustByRobotSiteRule(string custIds)
        {
            var criteria = new CustByRobotSiteRuleGetCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<int>(criteria);
        }

        public string GetExistedRobotUser(string custIds)
        {
            var criteria = new ExistedRobotUserGetCriteria
            {
                CustIds = custIds
            };

            repository.Execute(criteria);

            return criteria.RobotList;
        }
    }
}