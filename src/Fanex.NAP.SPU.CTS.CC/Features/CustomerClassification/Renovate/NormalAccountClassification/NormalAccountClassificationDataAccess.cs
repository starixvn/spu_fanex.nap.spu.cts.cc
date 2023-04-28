namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface INormalAccountClassificationDataAccess
    {
        GetNormalPoolCustomerModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentCustomerCategoryEntity> GetCurrentCustomerCategory(string custIDList);

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

        public IEnumerable<GetCurrentCustomerCategoryEntity> GetCurrentCustomerCategory(string custIDList)
        {
            var criteria = new GetCurrentCustomerCategoryCriteria
            {
                CustIDs = custIDList
            };

            return repository.Fetch<GetCurrentCustomerCategoryEntity>(criteria);
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
