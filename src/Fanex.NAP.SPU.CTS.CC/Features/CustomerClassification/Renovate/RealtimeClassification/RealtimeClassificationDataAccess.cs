namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRealtimeClassificationDataAccess
    {
        GetNewPlaceBetCustomerModel GetNewPlaceBetCustomers();

        GetCustomerCategoryRealtimeModel GetCustomerCategory(string custIDs);

        void PreprocessCustomerClassRealtime(IEnumerable<GetCustomerCategoryRealtimeEntity> custs);

        void InsertPADailyScanQueue(string custIDList);

        void CompleteCustomerRealtime(long scannedMaxId);

    }

    public class RealtimeClassificationDataAccess : IRealtimeClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public RealtimeClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNewPlaceBetCustomerModel GetNewPlaceBetCustomers()
        {
            var criteria = new GetChangeCustomerRealtimeCriteria();

            var data = repository.Fetch<GetNewPlaceBetCustomerEntity>(criteria);

            return new GetNewPlaceBetCustomerModel
            {
                CustomerNewPlaceBets = data,
                ScannedMaxId = criteria.ScannedMaxId
            };
        }

        public GetCustomerCategoryRealtimeModel GetCustomerCategory(string custIDs)
        {
            var criteria = new GetCustomerCategoryRealtimeCriteria
            {
                CustIDs = custIDs
            };

            var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());

            var data = db.QueryMultiple(criteria);

            return new GetCustomerCategoryRealtimeModel
            {
                CustomerCategory = data.Read<GetCustomerCategoryRealtimeEntity>()?.ToList() ??  new List<GetCustomerCategoryRealtimeEntity>(),
                CustomerPADailyScan = data.Read<long>()?.ToList() ?? new List<long>()
            };
        }

        public void PreprocessCustomerClassRealtime(IEnumerable<GetCustomerCategoryRealtimeEntity> custs)
        {
            var criteria = new PreprocessCustomerRealtimeCriteria
            {
                CustomersXML = custs.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void InsertPADailyScanQueue(string custIDList)
        {
            var criteria = new InsertQueuePADailyCriteria
            {
                CustIDList = custIDList
            };

            repository.Execute(criteria);
        }

        public void CompleteCustomerRealtime(long scannedMaxId)
        {
            var criteria = new CompleteCustomerRealtimeCriteria
            {
                ScannedMaxId = scannedMaxId
            };

            repository.Execute(criteria);
        }

    }
}
