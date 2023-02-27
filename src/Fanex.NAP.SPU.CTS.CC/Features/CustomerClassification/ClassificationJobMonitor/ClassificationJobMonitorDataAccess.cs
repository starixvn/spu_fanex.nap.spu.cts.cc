namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public interface IClassificationJobMonitorDataAccess
    {
        IEnumerable<long> GetRealtimeCustomerNewPlaceBet(DateTime fromScannedTime, DateTime toScannedTime);
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
    }
}