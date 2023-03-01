namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IClassificationJobMonitorDataAccess
    {
        IEnumerable<long> GetRealtimeCustomerNewPlaceBet(DateTime fromScannedTime, DateTime toScannedTime);

        IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds);

        void InsertMonitorIncorrectCustCategory(string custDataJson, JobMonitorType type);
    }

    public class ClassificationJobMonitorDataAccess : IClassificationJobMonitorDataAccess
    {
        private readonly IDynamicRepository repository;

        public ClassificationJobMonitorDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<long> GetRealtimeCustomerNewPlaceBet(DateTime fromScannedTime, DateTime toScannedTime)
        {
            var criteria = new RealtimeCustomerNewPlaceBetGetCriteria
            {
                FromScannedTime = fromScannedTime,
                ToScannedTime = toScannedTime
            };

            return repository.Fetch<long>(criteria);
        }

        public IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds)
        {
            var criteria = new CustomerCategoryMonitorGetCriteria()
            {
                CustIDs = custIds
            };

            return repository.Fetch<CustomerCategoriesMonitorEntity>(criteria);
        }

        public void InsertMonitorIncorrectCustCategory(string custDataJson, JobMonitorType type)
        {
            var criteria = new MonitorIncorrectCustCategoryInsertCriteria
            {
                CustomerListJson = custDataJson,
                InvalidType = (int)type
            };

            repository.Execute(criteria);
        }
    }
}