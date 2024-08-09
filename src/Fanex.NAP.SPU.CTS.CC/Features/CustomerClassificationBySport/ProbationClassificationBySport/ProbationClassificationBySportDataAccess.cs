namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface IProbationClassificationBySportDataAccess
    {
        ProbationBySportModel GetProbationScanClassificationBySport();

        void PreprocessProbationClassificationBySport(IEnumerable<ProbationScanClassificationBySportEntity> custList);
    }

    public class ProbationClassificationBySportDataAccess : IProbationClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProbationClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ProbationBySportModel GetProbationScanClassificationBySport()
        {
            var criteria = new GetProbationScanClassificationBySportCriteria();

            var data = repository.Fetch<ProbationScanClassificationBySportEntity>(criteria);

            return new ProbationBySportModel
            {
                CustProbations = data,
                LastCustID = criteria.LastCustID
            };
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
