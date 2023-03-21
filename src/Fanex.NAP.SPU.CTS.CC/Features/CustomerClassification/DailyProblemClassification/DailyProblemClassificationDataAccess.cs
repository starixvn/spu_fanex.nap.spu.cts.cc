namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface IDailyProblemClassificationDataAccess
    {
        IEnumerable<long> GetPADailyScanFromQueue();

        void CompletePADailyScan(string custIDs);

        IEnumerable<InsertPAProbationResult> InsertProblemProbationClassification(string problemAccountJson);
    }

    public class DailyProblemClassificationDataAccess : IDailyProblemClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyProblemClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<long> GetPADailyScanFromQueue()
        {
            var criteria = new PADailyScanFromQueueGetCriteria();

            return repository.Fetch<long>(criteria);
        }

        public IEnumerable<InsertPAProbationResult> InsertProblemProbationClassification(string problemAccountJson)
        {
            var criteria = new ProblemAccountProbationInsertCriteria
            {
                ProblemAccountJson = problemAccountJson
            };

            return repository.Fetch<InsertPAProbationResult>(criteria);
        }

        public void CompletePADailyScan(string custIDs)
        {
            var criteria = new PADailyScanCompleteCriteria
            {
                CustIDs = custIDs
            };

            repository.Execute(criteria);
        }
    }
}