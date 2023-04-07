namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface INormalAccountDataAccess
    {
        IEnumerable<CustomerClassificationEntity> GetNewClassification();

        IEnumerable<TaggedCustomerEntity> GetCustsAssociationProblemAccount(string custIds);

        InsertCategoryResultEntity InsertMainCustomerClassification(string newClassification);

        void UpdateAppliedStatusClassification(string custIdList);

        IEnumerable<TaggedCustomerEntity> CheckGroupBettingAndRejectCustomer(string custIds);
    }

    public class NormalAccountDataAccess : INormalAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustomerClassificationEntity> GetNewClassification()
        {
            var criteria = new NewClassificationGetCriteria();

            return repository.Fetch<CustomerClassificationEntity>(criteria);
        }

        public IEnumerable<TaggedCustomerEntity> GetCustsAssociationProblemAccount(string custIds)
        {
            var criteria = new TaggingByAssociationWithPAGetCriteria
            {
                CustIdList = custIds
            };

            return repository.Fetch<TaggedCustomerEntity>(criteria);
        }

        public IEnumerable<TaggedCustomerEntity> CheckGroupBettingAndRejectCustomer(string custIds)
        {
            var criteria = new CheckGroupBettingAndRejectCriteria
            {
                CustIdList = custIds
            };

            return repository.Fetch<TaggedCustomerEntity>(criteria);
        }

        public InsertCategoryResultEntity InsertMainCustomerClassification(string newClassification)
        {
            var criteria = new InsertMainCustomerClassificationCriteria
            {
                NewClassification = newClassification
            };

            var custIds = repository.Fetch<long>(criteria);

            return new InsertCategoryResultEntity { ErrorMessage = criteria.ErrorMessage, CustIds = custIds };
        }

        public void UpdateAppliedStatusClassification(string custIdList)
        {
            var criteria = new CustClassificationAppliedStatusUpdateCriteria
            {
                ListCustIds = custIdList,
            };

            repository.Execute(criteria);
        }
    }
}