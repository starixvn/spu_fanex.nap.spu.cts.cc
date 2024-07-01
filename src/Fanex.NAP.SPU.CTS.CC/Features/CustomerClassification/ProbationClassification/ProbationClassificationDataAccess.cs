namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using Fanex.Data.Repository;

    public interface IProbationClassificationDataAccess
    {
        ProbationModel GetProbationCustForScanning(int numberOfRecord);

        void ClassifyProbation(string custList);
    }

    public class ProbationClassificationDataAccess : IProbationClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProbationClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ProbationModel GetProbationCustForScanning(int numberOfRecord)
        {
            var criteria = new ProbationScanningGetCriteria
            {
                NumberOfRecord = numberOfRecord
            };

            var data = repository.Fetch<ProbationEntity>(criteria);

            return new ProbationModel
            {
                 CustProbations = data,
                 LastCustID = criteria.LastCustID,
                 LastCategoryID = criteria.LastCategoryID
            };
        }

        public void ClassifyProbation(string custList)
        {
            var criteria = new ProbationClassifCriteria
            {
                ListCustId = custList
            };

            repository.Execute(criteria);
        }
     
    }
}