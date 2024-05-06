namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;

    public interface IProbationClassificationDataAccess
    {
        IEnumerable<ProbationEntity> GetProbationCustForScanning(int numberOfRecord);

        void ClassifyProbation(string custList);

        void UpdateLastScanDate(string custSport);
    }

    public class ProbationClassificationDataAccess : IProbationClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProbationClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<ProbationEntity> GetProbationCustForScanning(int numberOfRecord)
        {
            var criteria = new ProbationScanningGetCriteria
            {
                NumberOfRecord = numberOfRecord
            };

            return repository.Fetch<ProbationEntity>(criteria);
        }

        public void ClassifyProbation(string custList)
        {
            var criteria = new ProbationClassifCriteria
            {
                ListCustId = custList
            };

            repository.Execute(criteria);
        }

        public void UpdateLastScanDate(string custSport)
        {
            var criteria = new ProbationScanningCompleteCriteria
            {
                CustSportJson = custSport
            };

            repository.Execute(criteria);
        }
    }
}