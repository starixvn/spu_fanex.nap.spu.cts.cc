namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IClassificationJobMonitorDataAccess
    {
        IEnumerable<long> GetRealtimeCustomerPlaceNewBet(DateTime fromScannedTime, DateTime toScannedTime);

        void ClearRealtimeCustomerPlaceNewBets(DateTime fromScannedTime, DateTime toScannedTime);

        IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds, bool isCheckInternalAccount = false);

        void InsertMonitorIncorrectCustCategory(string custDataJson, DateTime scannedTime, JobMonitorType type);
    }

    public class ClassificationJobMonitorDataAccess : IClassificationJobMonitorDataAccess
    {
        private readonly IDynamicRepository repository;

        public ClassificationJobMonitorDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<long> GetRealtimeCustomerPlaceNewBet(DateTime fromScannedTime, DateTime toScannedTime)
        {
            var criteria = new RealtimeCustomerNewPlaceBetGetCriteria
            {
                FromScannedTime = fromScannedTime,
                ToScannedTime = toScannedTime
            };

            return repository.Fetch<long>(criteria);
        }

        public void ClearRealtimeCustomerPlaceNewBets(DateTime fromScannedTime, DateTime toScannedTime)
        {
            var criteria = new RealtimeCustomerNewPlaceBetClearCriteria
            {
                FromScannedTime = fromScannedTime,
                ToScannedTime = toScannedTime
            };

            repository.Execute(criteria);
        }

        public IEnumerable<CustomerCategoriesMonitorEntity> GetCustomerCategoriesToMonitor(string custIds, bool isCheckInternalAccount)
        {
            var criteria = new CustomerCategoryMonitorGetCriteria()
            {
                CustIDs = custIds,
                IsCheckInternalAccount = isCheckInternalAccount
            };

            return repository.Fetch<CustomerCategoriesMonitorEntity>(criteria);
        }

        public void InsertMonitorIncorrectCustCategory(string custDataJson, DateTime scannedTime, JobMonitorType type)
        {
            var criteria = new MonitorIncorrectCustCategoryInsertCriteria
            {
                CustomerListJson = custDataJson,
                ScannedTime =  scannedTime,
                InvalidType = (int)type
            };

            repository.Execute(criteria);
        }
    }
}