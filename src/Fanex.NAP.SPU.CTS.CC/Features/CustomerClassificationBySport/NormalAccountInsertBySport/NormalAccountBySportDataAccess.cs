namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountInsertBySport
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface INormalAccountBySportDataAccess
    {
        IEnumerable<CustomerClassificationBySportEntity> GetNewClassificationBySport();

        InsertNormalAccountResultEntity InsertCustomerClassificationBySport(string newClassification);

        void UpdateAppliedStatusClassificationBySport(string custIdList);
    }

    public class NormalAccountBySportDataAccess : INormalAccountBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public NormalAccountBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustomerClassificationBySportEntity> GetNewClassificationBySport()
        {
            var criteria = new GetNewClassificationBySportCriteria();

            return repository.Fetch<CustomerClassificationBySportEntity>(criteria);
        }

        public InsertNormalAccountResultEntity InsertCustomerClassificationBySport(string newClassification)
        {
            var criteria = new InsertNormalAccountBySportCriteria
            {
                NewClassification = newClassification
            };

            var custInfos = repository.Fetch<InsertNormalAccountCustInfoEntity>(criteria);

            return new InsertNormalAccountResultEntity { ErrorMessage = criteria.ErrorMessage, CustInfos = custInfos };
        }

        public void UpdateAppliedStatusClassificationBySport(string custs)
        {
            var criteria = new UpdateAppliedStatusClassificationBySportCriteria
            {
                CustomersXML = custs
            };

            repository.Execute(criteria);
        }

    }
}
