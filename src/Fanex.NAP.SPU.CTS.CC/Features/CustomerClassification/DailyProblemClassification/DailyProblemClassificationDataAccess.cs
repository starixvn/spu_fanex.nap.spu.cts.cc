namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface IDailyProblemClassificationDataAccess
    {
        PADailyScanFromQueueModel GetPADailyScanFromQueue();

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

        public PADailyScanFromQueueModel GetPADailyScanFromQueue()
        {
            var criteria = new PADailyScanFromQueueGetCriteria();

            var (firstResultSet, secondResultSet) = repository.FetchMultiple<ulong, CustToBuildRemarkEntity>(criteria);

            return new PADailyScanFromQueueModel
            {
                PADailyScanCustIDs = firstResultSet,
                PAPotentialScanCustIDs = secondResultSet
            };
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