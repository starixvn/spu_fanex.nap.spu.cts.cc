using Fanex.NAP.SPU.CTS.Features.ExistingCustomer;
using Fanex.NAP.SPU.CTS.Features.ExistingCustomer.Entities;
using Fanex.NAP.SPU.CTS.Shares;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    public interface IExistingCustomerJobService
    {
        IEnumerable<CustomerEntity> GetPlaceBetCustomer(DateTime fromDate, DateTime toDate);

        IEnumerable<CTSCustomerEntity> FoundCustomerNoTransInCTS(IEnumerable<CustomerEntity> customers, DateTime fromDate, DateTime toDate);
    }

    public class ExistingCustomerJobService : IExistingCustomerJobService
    {
        private readonly IExistingCustomerServices existingCustomerServices;
        private readonly ISentryClient sentryClient;        

        public ExistingCustomerJobService(IExistingCustomerServices existingCustomerServices,
            ISentryClient sentryClient)
        {
            this.existingCustomerServices = existingCustomerServices;
            this.sentryClient = sentryClient;            
        }

        public IEnumerable<CustomerEntity> GetPlaceBetCustomer(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return existingCustomerServices.GetPlaceBetCustomer(fromDate, toDate);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
            }

            return new List<CustomerEntity>();
        }

        public IEnumerable<CTSCustomerEntity> FoundCustomerNoTransInCTS(IEnumerable<CustomerEntity> customers, DateTime fromDate, DateTime toDate)
        {
            var foundCustomerNotTrans = new List<CTSCustomerEntity>();

            try
            {
                if (customers != null && customers.Any())
                {
                    var chunkCustomers = customers.Chunk2(100);

                    foreach(var customerChunk in chunkCustomers)
                    {
                        var custIds = customerChunk.Select(x => x.CustId).Distinct();                                                

                        CheckingExistedCustomerInCTS(fromDate, toDate, foundCustomerNotTrans, custIds);
                    }                    
                }
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
            }

            return foundCustomerNotTrans;
        }

        private void CheckingExistedCustomerInCTS(DateTime fromDate, DateTime toDate, List<CTSCustomerEntity> foundCustomerNotTrans, IEnumerable<int> custIds)
        {
            var custIdString = string.Join(",", custIds);

            var result = existingCustomerServices.CheckCustomerNotExisted(custIdString, fromDate, toDate);

            if (result.Any())
            {
                foundCustomerNotTrans.AddRange(result);
            }
        }
    }
}
