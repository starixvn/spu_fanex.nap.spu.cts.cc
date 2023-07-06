namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface IProbationClassificationBySportDataAccess
    {
        IEnumerable<ProbationScanClassificationBySportEntity> GetProbationScanClassificationBySport(int numberOfRecord);

        void PreprocessProbationClassificationBySport(IEnumerable<ProbationScanClassificationBySportEntity> custList);
    }

    public class ProbationClassificationBySportDataAccess : IProbationClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProbationClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<ProbationScanClassificationBySportEntity> GetProbationScanClassificationBySport(int numberOfRecord)
        {
            var criteria = new GetProbationScanClassificationBySportCriteria
            {
                NumberOfRecord = numberOfRecord
            };

            return repository.Fetch<ProbationScanClassificationBySportEntity>(criteria);
        }

        public void PreprocessProbationClassificationBySport(IEnumerable<ProbationScanClassificationBySportEntity> custList)
        {
            var criteria = new PreprocessProbationClassificationBySportCriteria
            {
                CustomersXML = custList.ToXmlString()
            };

            repository.Execute(criteria);
        }
    }
}
