namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.WrapperCustomerClassification;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalAccountDataAccess
    {
        GetNormalAccountModel GetNormalAccount();

        IEnumerable<CTSTaggedCustEntity> GetCustsAssociationProblemAccount(string custIdList);

        IEnumerable<TWTaggedCustEntity> CheckGroupBettingAndRejectCustomer(string custIdList);      

        void CompleteNormalAccountInsert(long scannedMaxId);

        ResultInsertCustomerClassificationModel InsertNormalCustomerClassification(InsertCustomerClassificationModel model);
    }

    public class NormalAccountDataAccess : WrapperCustomerClassificationDataAccess, INormalAccountDataAccess
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

        public void CompleteNormalAccountInsert(long scannedMaxId)
        {
            var criteria = new InsertCompleteNormalAccountCrieria
            {
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }

        public ResultInsertCustomerClassificationModel InsertNormalCustomerClassification(InsertCustomerClassificationModel model)
        {
            return base.InsertCustomerClassification(CCInsertInputFlow.Normal, model);
        }
       
    }
}
