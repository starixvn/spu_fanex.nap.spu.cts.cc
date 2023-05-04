namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface INormalAccountDataAccess
    {
        IEnumerable<GetNormalAccountEntity> GetNormalAccount();

        IEnumerable<CTSTaggedCustEntity> GetCustsAssociationProblemAccount(string custIdList);

        IEnumerable<TWTaggedCustEntity> CheckGroupBettingAndRejectCustomer(string custIdList);

        InsertNormalAccountResultEntity InsertNormalAccount(string custInfoList);

        void UpdateCompleteNormalAccountInsert(string custIdList);

    }

    public class NormalAccountDataAccess : INormalAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetNormalAccountEntity> GetNormalAccount()
        {
            var criteria = new GetNormalAccountCriteria();

            return repository.Fetch<GetNormalAccountEntity>(criteria);
        }

        public IEnumerable<CTSTaggedCustEntity> GetCustsAssociationProblemAccount(string custIdList)
        {
            var criteria = new TaggingByAssociationWithPAGetCriteria
            {
                CustIdList = custIdList
            };

            return repository.Fetch<CTSTaggedCustEntity>(criteria);
        }

        public IEnumerable<TWTaggedCustEntity> CheckGroupBettingAndRejectCustomer(string custIdList)
        {
            var criteria = new CheckGroupBettingAndRejectCriteria
            {
                CustIdList = custIdList
            };

            return repository.Fetch<TWTaggedCustEntity>(criteria);
        }


        public InsertNormalAccountResultEntity InsertNormalAccount(string custIdList)
        {
            var criteria = new InsertNormalAccountCriteria
            {
                NewClassification = custIdList
            };

            var custIds = repository.Fetch<long>(criteria);

            return new InsertNormalAccountResultEntity { CustIds = custIds, ErrorMessage = criteria.ErrorMessage };
        }

        public void UpdateCompleteNormalAccountInsert(string custIdList)
        {
            var criteria = new InsertCompleteNormalAccountCrieria
            {
                ListCustIds = custIdList
            };

            repository.Execute(criteria);
        }

    }
}
