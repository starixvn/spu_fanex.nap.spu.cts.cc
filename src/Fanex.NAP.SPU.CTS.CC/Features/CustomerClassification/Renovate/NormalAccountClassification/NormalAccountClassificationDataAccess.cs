namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface INormalAccountClassificationDataAccess
    {
        GetNormalPoolCustomerModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(string custIDList);

        void ClassifyNormalPoolCustomers(string customerXML, bool isScanOnlyTagging);

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

        public IEnumerable<GetCurrentNormalCategoryEntity> GetCurrentCustomerCategory(string custIDList)
        {
            var criteria = new GetCurrentNormalCategoryCriteria
            {
                CustIDs = custIDList
            };

            return repository.Fetch<GetCurrentNormalCategoryEntity>(criteria);
        }

        public void ClassifyNormalPoolCustomers(string customerXML, bool isScanOnlyTagging)
        {
            var criteria = new ClassifyNormalPoolCustomerCriteria
            {
                 CustomersXML = customerXML,
                 IsScanOnlyTagging = isScanOnlyTagging
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
