namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface INormalAccountClassificationDataAccess
    {
        GetNormalPoolCustomerModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(string custInfos);

        void ClassifyNormalPoolCustomers(string customerXML);

        void ClearNormalPoolCustomers(string custIdList, long scannedMaxId);
    }

    public class NormalAccountClassificationDataAccess : INormalAccountClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNormalPoolCustomerModel GetNormalPoolCustomers()
        {
            var criteria = new GetNormalPoolCustomerCriteria();

            var data = repository.Fetch<GetNormalPoolCustomerEntity>(criteria);

            return new GetNormalPoolCustomerModel
            {
                NormalPoolCustomers = data,
                ScannedMaxId = criteria.ScannedMaxId
            };
        }

        public IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(string custInfos)
        {
            var criteria = new GetCurrentNormalCategoryCriteria
            {
                CustInfos = custInfos
            };

            return repository.Fetch<GetCurrentNormalCategoryEntity>(criteria);
        }

        public void ClassifyNormalPoolCustomers(string customerXML)
        {
            var criteria = new ClassifyNormalPoolCustomerCriteria
            {
                 CustomersXML = customerXML
            };

            repository.Execute(criteria);
        }

        public void ClearNormalPoolCustomers(string custIdList, long scannedMaxId)
        {
            var criteria = new ClearNormalPoolCustomerCriteria
            {
                ListCustId = custIdList,
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }
    }
}
