namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public interface IClassificationJobMonitorDataAccess
    {
        IEnumerable<long> GetRealtimeCustomerPlaceNewBet(DateTime fromScannedTime, DateTime toScannedTime);

        void ClearRealtimeCustomerPlaceNewBets(DateTime fromScannedTime, DateTime toScannedTime);

        IEnumerable<CustomerCategoryMonitorEntity> GetCustomerCategoryToMonitor(string custIds, bool isCheckInternalAccount);

        NormalClassificationPoolMonitorEntity GetNormalClassificationPoolMonitor();
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

        public IEnumerable<CustomerCategoryMonitorEntity> GetCustomerCategoryToMonitor(string custIds, bool isCheckInternalAccount)
        {
            var criteria = new CustomerCategoryMonitorGetCriteria()
            {
                CustIDs = custIds,
                IsCheckInternalAccount = isCheckInternalAccount
            };

            return repository.Fetch<CustomerCategoryMonitorEntity>(criteria);
        }

        public NormalClassificationPoolMonitorEntity GetNormalClassificationPoolMonitor()
        {
            var criteria = new NormalClassificationPoolMonitorGetCriteria();

            repository.Execute(criteria);

            return new NormalClassificationPoolMonitorEntity()
            {
                GeneralRecord = criteria.GeneralRecord,
                BySportRecord = criteria.BySportRecord
            };
        }
    }
}