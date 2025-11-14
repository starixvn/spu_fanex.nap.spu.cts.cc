namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport.Model;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemWLClassificationBySportDataAccess
    {
        IEnumerable<ProblemAccountInfoFromDailyQueueModel> GetPADailyFromQueue(int batchSize);

        void CompletePADailyFromQueue(string custInfos);

        IEnumerable<InsertPAProbationBySportResultModel> InsertPAWLPerformanceBySport(string paJson);
    }

    public class ProblemWLClassificationBySportDataAccess : IProblemWLClassificationBySportDataAccess
    {
        private readonly IDynamicRepository _repository;

        public ProblemWLClassificationBySportDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<ProblemAccountInfoFromDailyQueueModel> GetPADailyFromQueue(int batchSize)
        {
            var criteria = new PADailyScanFromQueueBySportGetCriteria
            {
                BatchSize = batchSize,
            };

            var entities = _repository.Fetch<ProblemAccountInfoFromDailyQueueEntity>(criteria);

            return entities.Select(x => new ProblemAccountInfoFromDailyQueueModel(x));
        }

        public void CompletePADailyFromQueue(string custInfos)
        {
            var criteria = new PADailyScanFromQueueBySportCompleteCriteria
            {
                CustInfoJson = custInfos
            };

            _repository.Execute(criteria);
        }

        public IEnumerable<InsertPAProbationBySportResultModel> InsertPAWLPerformanceBySport(string paJson)
        {
            var criteria = new ProblemAccountProbationBySportInsertCriteria
            {
                ProblemAccountJson = paJson
            };

            var entities = _repository.Fetch<InsertPAProbationBySportResultEntity>(criteria);

            return entities.Select(x => new InsertPAProbationBySportResultModel(x));
        }
    }
}
