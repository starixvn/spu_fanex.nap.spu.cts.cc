namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    public interface INormalAccountDataAccess
    {
        GetNormalAccountModel GetNormalAccount(bool isLogData);

        IEnumerable<CTSTaggedCustEntity> GetCustsAssociationProblemAccount(string custIdList);

        IEnumerable<TWTaggedCustEntity> CheckGroupBettingAndRejectCustomer(string custIdList);      

        void CompleteNormalAccountInsert(long scannedMaxId);

        IEnumerable<TWTaggedExcludeEntity> CheckTWTaggingExclude(IEnumerable<TWTaggedExcludeModel> taggedEntities);

        IEnumerable<CTSTaggedCustEntity> GetTWTaggedCustBySpecialLicSub(string custIdList);

        void CompleteSpecialLicSubClassificationComplete(string custIdList);
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
       
        public IEnumerable<TWTaggedExcludeEntity> CheckTWTaggingExclude(IEnumerable<TWTaggedExcludeModel> taggedEntities)
        {
            if (taggedEntities.IsNullOrEmpty())
            {
                return Enumerable.Empty<TWTaggedExcludeEntity>();
            }

            var criteria = new CheckTWTaggingExcludeCriteria
            {
                JsonData = JsonSerializer.Serialize(taggedEntities)
            };

            return repository.Fetch<TWTaggedExcludeEntity>(criteria);
        }

        public IEnumerable<CTSTaggedCustEntity> GetTWTaggedCustBySpecialLicSub(string custIdList)
        {
            var criteria = new CheckTWSpecialLicSubCriteria
            {
                CustIds = custIdList
            };

            return repository.Fetch<CTSTaggedCustEntity>(criteria);
        }

        public void CompleteSpecialLicSubClassificationComplete(string custIdList)
        {
            var criteria = new CompleteSpecialLicSubClassifyCriteria
            {
                CustIDs = custIdList
            };

            repository.Execute(criteria);
        }
    }
}
