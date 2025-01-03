namespace Fanex.NAP.SPU.CTS.CC.Features.JobMonitoring.IncorrectDataMonitoring
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IIncorrectDataMonitoringDataAccess
    {
        GetNotificationMonitoringEntity GetIncorrectDataMonitoring();

        void InsertIncorrectDataMonitoring(string incorrectDataJson, JobMonitor_IssueType type);
    }

    public class IncorrectDataMonitoringDataAccess : IIncorrectDataMonitoringDataAccess
    {
        private readonly IDynamicRepository repository;

        public IncorrectDataMonitoringDataAccess()
        {
            repository = new DynamicRepository();
        }

        public GetNotificationMonitoringEntity GetIncorrectDataMonitoring()
        {
            var criteria = new NotificationMonitoringGetCriteria();

            var invalidDataList = repository.Fetch<GetInvalidDataMonitoringEntity>(criteria);

            return new GetNotificationMonitoringEntity
            {
                IsContinue = criteria.IsContinue,
                InvalidDataList = invalidDataList ?? new List<GetInvalidDataMonitoringEntity>()
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S927:Parameter names should match base declaration and other partial definitions", Justification = "<Pending>")]
        public void InsertIncorrectDataMonitoring(string incorrectDataJson, JobMonitor_IssueType issueType)
        {
            var criteria = new NotificationMonitoringInsertCriteria
            {
                IncorrectData = incorrectDataJson,
                IssueTypeID = (int)issueType
            };

            repository.Execute(criteria);
        }
    }
}