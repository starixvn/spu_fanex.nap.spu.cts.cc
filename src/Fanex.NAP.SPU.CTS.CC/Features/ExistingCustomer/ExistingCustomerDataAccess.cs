using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.NAP.SPU.CTS.Features.ExistingCustomer.Criteria;
using Fanex.NAP.SPU.CTS.Features.ExistingCustomer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.CTS.Features.ExistingCustomer
{
    public interface IExistingCustomerDataAccess
    {
        IEnumerable<CustomerEntity> GetPlaceBetCustomers(DateTime fromDate, DateTime toDate);

        IEnumerable<CTSCustomerEntity> CheckCustomerNotExisted(string custIds, DateTime fromDate, DateTime toDate);
    }


    public class ExistingCustomerDataAccess : IExistingCustomerDataAccess
    {
        private readonly IDynamicRepository repository;

        public ExistingCustomerDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustomerEntity> GetPlaceBetCustomers(DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetPlaceBetCustomersCriteria
            {
               FromDate = fromDate,
               ToDate = toDate
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerEntity>(criteria);
            }
        }

        public IEnumerable<CTSCustomerEntity> CheckCustomerNotExisted(string custIds, DateTime fromDate, DateTime toDate)
        {
            var criteria = new CheckCustNotExistedByCustIdsCriteria
            {
                CustIds = custIds,
                FromDate = fromDate,
                ToDate = toDate
            };

            return repository.Fetch<CTSCustomerEntity>(criteria);
        }
    }
}
