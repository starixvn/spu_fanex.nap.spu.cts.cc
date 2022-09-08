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

        InsertNormalAccountResultEntity InsertCustomerClassification(string newClassification);
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

        public InsertNormalAccountResultEntity InsertCustomerClassification(string newClassification)
        {
            var criteria = new InsertNormalAccountBySportCriteria
            {
                NewClassification = newClassification
            };

            var custIDs = repository.Fetch<long>(criteria);

            return new InsertNormalAccountResultEntity { ErrorMessage = criteria.ErrorMessage, CustIDs = custIDs };
        }

    }
}
