namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;       

    public interface IProbationClassificationBySportDataAccess
    {
        IEnumerable<ProbationClassificationBySportEntity> GetProbationCustClassificationBySport(int numberOfRecord);

        void ScanProbationClassificationBySport(IEnumerable<ScanProbationClassificationBySportEntity> custList);

        void UpdateScanDateProbationClassificationBySport(string custSport);
    }

    public class ProbationClassificationBySportDataAccess : IProbationClassificationBySportDataAccess
    {
        private readonly IDynamicRepository repository;

        public ProbationClassificationBySportDataAccess()
        {
            repository = new DynamicRepository();
        }
       
        public IEnumerable<ProbationClassificationBySportEntity> GetProbationCustClassificationBySport(int numberOfRecord)
        {
            var criteria = new GetProbationCustClassificationBySportCriteria
            {
                NumberOfRecord = numberOfRecord
            };

            return repository.Fetch<ProbationClassificationBySportEntity>(criteria);
        }

        public void ScanProbationClassificationBySport(IEnumerable<ScanProbationClassificationBySportEntity> custList)
        {
            var criteria = new ScanProbationClassificationBySportCriteria
            {
                CustomersXML = custList.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void UpdateScanDateProbationClassificationBySport(string custSport)
        {
            var criteria = new UpdateScanDateProbationClassificationBySportCriteria
            {
                CustSportJson = custSport
            };

            repository.Execute(criteria);
        }
    }
}
