namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface INormalAccountDataAccess
    {
        IEnumerable<CustomerClassificationEntity> GetNewClassification();

        IEnumerable<CustAssociationProblemAccountEntity> GetCustsAssociationProblemAccount(string custIds);

        InsertCategoryResultEntity InsertMainCustomerClassification(string newClassification);

        void UpdateAppliedStatusClassification(string custIdList, bool isGeneralCategory);
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

        public IEnumerable<CustAssociationProblemAccountEntity> GetCustsAssociationProblemAccount(string custIds)
        {
            var criteria = new TaggingByAssociationWithPAGetCriteria
            {
                CustIdList = custIds
            };

            return repository.Fetch<CustAssociationProblemAccountEntity>(criteria);
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

        public void UpdateAppliedStatusClassification(string custIdList, bool isGeneralCategory)
        {
            var criteria = new CustClassificationAppliedStatusUpdateCriteria
            {
                ListCustIds = custIdList,
                IsGeneralCategory = isGeneralCategory
            };

            repository.Execute(criteria);
        }
    }
}