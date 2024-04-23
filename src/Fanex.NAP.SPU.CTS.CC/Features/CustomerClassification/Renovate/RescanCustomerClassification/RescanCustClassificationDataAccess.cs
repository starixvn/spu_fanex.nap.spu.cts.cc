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

        void ClassifyNormalCategory(string custList, FunctionId functionId, RescanNormalType type);

        IEnumerable<RescanPADangerousEntity> RescanPADangerous(string custIDs);

        IEnumerable<GetInitialTWGroupBettingEntity> GetInitialTWGroupBetting(string custIDs);

        IEnumerable<PotentialPACustInfoEntity> GetCustInfos(string custIDs);
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

        public void ClassifyNormalCategory(string custList, FunctionId functionId, RescanNormalType type)
        {
            var criteria = new NormalClassificationRescanCriteria
            {
                ListCustId = custList,
                FunctionId = (int)functionId,
                RescanTypeId = (int)type

            };

            repository.Execute(criteria);
        }

        public IEnumerable<RescanPADangerousEntity> RescanPADangerous(string custIDs)
        {
            var criteria = new RescanPADangerousCriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<RescanPADangerousEntity>(criteria);
        }

        public IEnumerable<GetInitialTWGroupBettingEntity> GetInitialTWGroupBetting(string custIDs)
        {
            var criteria = new RescanInitialTWGroupBettingCriteria() { CustIDs = custIDs };

            return repository.Fetch<GetInitialTWGroupBettingEntity>(criteria);
        }

        public IEnumerable<PotentialPACustInfoEntity> GetCustInfos(string custIDs)
        {
            var criteria = new GetCustInfoByCustIDCriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<PotentialPACustInfoEntity>(criteria);
        }

    }
}