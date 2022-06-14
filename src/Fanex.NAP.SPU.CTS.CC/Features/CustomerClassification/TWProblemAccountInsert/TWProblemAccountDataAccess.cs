namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Entities;    
    using System.Collections.Generic;

    public interface ITWProblemAccountDataAccess
    {
        IEnumerable<RescanTWProblemAccountModel> ScanSystemFormulaBetMonitor(out uint lastCustID);

        IEnumerable<RescanTWProblemAccountModel> GetCustomerPerformance(string custIdList);

        void RemoveSystemFormulaBetCustomer(string custIdList);    
        
        void ScanSystemFormulaBetMonitorComplete(uint lastCustId);
    }

    public class TWProblemAccountDataAccess : ITWProblemAccountDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWProblemAccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RescanTWProblemAccountModel> ScanSystemFormulaBetMonitor(out uint lastCustID)
        {
            var criteria = new ScanSystemFormulaBetCustomerCriteria();
            var result = repository.Fetch<RescanTWProblemAccountModel>(criteria);
            lastCustID = criteria.LastCustId;
            return result;
        }

        public IEnumerable<RescanTWProblemAccountModel> GetCustomerPerformance(string custIdList)
        {
            var criteria = new GetCustomerPerformanceCriteria
            {
                CustIdList = custIdList
            };

            return repository.Fetch<RescanTWProblemAccountModel>(criteria);
        }

        public void RemoveSystemFormulaBetCustomer(string custIdList)
        {
            var criteria = new RemoveSystemFormulaBetCustomerCriteria
            {
                CustIdList = custIdList
            };

            repository.Execute(criteria);
        }

        public void ScanSystemFormulaBetMonitorComplete(uint lastCustId)
        {
            var criteria = new ScanSystemFormulaBetCustomerCompleteCriteria
            {
                LastCustId = lastCustId
            };

            repository.Execute(criteria);
        }
        
    }
}
