namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport.Criteria;
    using System;
    using System.Collections.Generic;

    public interface INormalAccountClassificationBySportDataAccess
    {
        GetNormalPoolCustomerBySportModel GetNormalPoolCustomers();

        IEnumerable<GetCurrentNormalCategoryBySportEntity> GetCurrentCustomerCategory(string custSportList);

        void ClassifyNormalPoolCustomers(string customersXML);

        void ClearNormalPoolCustomers(string customersXML, long scannedMaxId);
    }

    public class NormalAccountClassificationBySportDataAccess : INormalAccountClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNormalPoolCustomerBySportModel GetNormalPoolCustomers()
        {
            var criteria = new GetNormalPoolCustomerBySportCriteria();

            var data = repository.Fetch<GetNormalPoolCustomerBySportEntity>(criteria);

            return new GetNormalPoolCustomerBySportModel
            {
                NormalPoolCustomers = data,
                ScannedMaxId = criteria.ScannedMaxId
            };
        }

        public IEnumerable<GetCurrentNormalCategoryBySportEntity> GetCurrentCustomerCategory(string custSportList)
        {
            var criteria = new GetCurrentNormalCategoryBySportCriteria
            {
                CustSportList = custSportList
            };

            return repository.Fetch<GetCurrentNormalCategoryBySportEntity>(criteria);
        }

        public void ClassifyNormalPoolCustomers(string customersXML)
        {
            var criteria = new ClassifyNormalPoolCustomerBySportCriteria
            {
                CustomersXML = customersXML
            };

            repository.Execute(criteria);
        }

        public void ClearNormalPoolCustomers(string customersXML, long scannedMaxId)
        {
            var criteria = new ClearNormalPoolCustomerBySportCriteria
            {
                CustomersXML = customersXML,
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }
    }
}
