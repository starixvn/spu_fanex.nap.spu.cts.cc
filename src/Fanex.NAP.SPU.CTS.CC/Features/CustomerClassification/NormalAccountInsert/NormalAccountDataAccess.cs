namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalAccountDataAccess
    {
        GetNormalAccountModel GetNormalAccount();

        IEnumerable<CTSTaggedCustEntity> GetCustsAssociationProblemAccount(string custIdList);

        IEnumerable<TWTaggedCustEntity> CheckGroupBettingAndRejectCustomer(string custIdList);

        IEnumerable<NewTWTaggedCustEntity> CheckNewGroupBetting(string custIdList);

        InsertNormalAccountResultEntity InsertNormalAccount(string custInfoList);

        void CompleteNormalAccountInsert(long scannedMaxId);
    }

    public class NormalAccountDataAccess : INormalAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNormalAccountModel GetNormalAccount()
        {
            var criteria = new GetNormalAccountCriteria();

            var data = repository.Fetch<GetNormalAccountEntity>(criteria);

            return new GetNormalAccountModel
            {
                NormalAccount = data,
                ScannedMaxId = criteria.ScannedMaxId,
                IsContinute = data != null && data.Any()
            };
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

        public void CompleteNormalAccountInsert(long scannedMaxId)
        {
            var criteria = new InsertCompleteNormalAccountCrieria
            {
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }

        public IEnumerable<NewTWTaggedCustEntity> CheckNewGroupBetting(string custIdList)
        {
            var criteria = new CheckNewGroupBettingCriteria()
            {
                CustIdList = custIdList
            };

            return repository.Fetch<NewTWTaggedCustEntity>(criteria);
        }
    }
}
