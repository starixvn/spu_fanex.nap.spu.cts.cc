namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Criteria;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRealtimeClassificationDataAccess
    {
        void CheckNewChangesForRealtime();

        IEnumerable<RealtimeNewPlaceBetCustomerEntity> GetNewPlaceBetCustomers();

        CustomerCategoryRealtimeViewEntity GetCustomerCategory(string custIDs);

        void ClassifyRealtimePerformance(IEnumerable<CustomerCategoryRealtimeEntity> customers);

        void UpdateGeneralRealtimeLastScan(bool isGeneralFlow);

        void UpdateAssociationCustomerLastTicket(string custIdList);

        void InsertPADailyScanQueue(string custIdList);
    }

    public class RealtimeClassificationDataAccess : IRealtimeClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public RealtimeClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void CheckNewChangesForRealtime()
        {
            var criteria = new NewChangesRealtimeCheckCriteria();

            repository.Execute(criteria);
        }

        public IEnumerable<RealtimeNewPlaceBetCustomerEntity> GetNewPlaceBetCustomers()
        {
            var criteria = new CustomerNewPlaceBetRealtimeGetCriteria();

            return repository.Fetch<RealtimeNewPlaceBetCustomerEntity>(criteria);
        }

        public CustomerCategoryRealtimeViewEntity GetCustomerCategory(string custIDs)
        {
            var criteria = new CustomerCategoryRealtimeGetCriteria
            {
                CustIDs = custIDs
            };

            var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());

            var result = db.QueryMultiple(criteria);

            return new CustomerCategoryRealtimeViewEntity()
            {
                CustCategoryRealtime = result?.Read<CustomerCategoryRealtimeEntity>() ?? Enumerable.Empty<CustomerCategoryRealtimeEntity>(),
                CustPADailyScan = result?.Read<long>() ?? Enumerable.Empty<long>()
            };
        }

        public void ClassifyRealtimePerformance(IEnumerable<CustomerCategoryRealtimeEntity> customers)
        {
            var criteria = new NormalAccountsClassificationGeneralCategorizeRealtimeCriteria
            {
                CustomersXML = customers.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void UpdateGeneralRealtimeLastScan(bool isGeneralFlow)
        {
            var criteria = new LastScanRealtimeUpdateCriteria
            {
                IsGeneralFlow = isGeneralFlow
            };

            repository.Execute(criteria);
        }

        public void UpdateAssociationCustomerLastTicket(string custIdList)
        {
            var criteria = new UpdateAssociationCustomerLastTicketCriteria
            {
                CustIdList = custIdList
            };

            repository.Execute(criteria);
        }

        public void InsertPADailyScanQueue(string custIdList)
        {
            var criteria = new QueuePADailyInsertCriteria
            {
                CustIDs = custIdList
            };

            repository.Execute(criteria);
        }
    }
}