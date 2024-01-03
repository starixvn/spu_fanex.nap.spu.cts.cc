namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Fanex.Data.Repository;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IIncorrectDataMonitoringDataAccess
    {
        public IEnumerable<GetMonitoringEntity> GetIncorrectDataMonitoring();

        void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> dataJson, JobMonitorType type);
    }

    public class IncorrectDataMonitoringDataAccess : IIncorrectDataMonitoringDataAccess
    {
        private readonly IDynamicRepository repository;

        public IncorrectDataMonitoringDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<GetMonitoringEntity> GetIncorrectDataMonitoring()
        {
            var criteria = new JobMonitoringGetCriteria();

            return repository.Fetch<GetMonitoringEntity>(criteria);
        }

        public void InsertIncorrectDataMonitoring(IEnumerable<InsertMonitoringEntity> incorrectData, JobMonitorType issueType)
        {
            var criteria = new JobMonitoringInsertCriteria
            {
                IncorrectData = JsonConvert.SerializeObject(incorrectData),
                IssueTypeID = (int)issueType
            };

            repository.Execute(criteria);
        }
    }
}