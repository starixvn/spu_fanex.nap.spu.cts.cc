namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalAccountDataAccess
    {
        GetNormalAccountModel GetNormalAccount(bool isLogData);

        IEnumerable<CTSTaggedCustEntity> GetCustsAssociationProblemAccount(string custIdList);

        IEnumerable<TWTaggedCustEntity> CheckGroupBettingAndRejectCustomer(string custIdList);      

        void CompleteNormalAccountInsert(long scannedMaxId);

    }

    public class NormalAccountDataAccess : INormalAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNormalAccountModel GetNormalAccount(bool isLogData)
        {
            var criteria = new GetNormalAccountCriteria
            { 
               IsLogData = isLogData
            };

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

        public void CompleteNormalAccountInsert(long scannedMaxId)
        {
            var criteria = new InsertCompleteNormalAccountCrieria
            {
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }        
       
    }
}
