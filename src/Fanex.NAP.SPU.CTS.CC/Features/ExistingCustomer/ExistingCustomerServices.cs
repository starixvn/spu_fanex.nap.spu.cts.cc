using Fanex.NAP.SPU.CTS.Features.ExistingCustomer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.CTS.Features.ExistingCustomer
{
    public interface IExistingCustomerServices
    {
        IEnumerable<CustomerEntity> GetPlaceBetCustomer(DateTime fromDate, DateTime toDate);

        IEnumerable<CTSCustomerEntity> CheckCustomerNotExisted(string custIds, DateTime fromDate, DateTime toDate);
    }
    public class ExistingCustomerServices : IExistingCustomerServices
    {
        private readonly IExistingCustomerDataAccess existingCustomerDataAccess;

        public ExistingCustomerServices(IExistingCustomerDataAccess existingCustomerDataAccess)
        {
            this.existingCustomerDataAccess = existingCustomerDataAccess;
        }

        public IEnumerable<CTSCustomerEntity> CheckCustomerNotExisted(string custIds, DateTime fromDate, DateTime toDate)
        {
            return this.existingCustomerDataAccess.CheckCustomerNotExisted(custIds, fromDate, toDate);
        }

        public IEnumerable<CustomerEntity> GetPlaceBetCustomer(DateTime fromDate, DateTime toDate)
        {
            return existingCustomerDataAccess.GetPlaceBetCustomers(fromDate, toDate);
        }
    }
}
