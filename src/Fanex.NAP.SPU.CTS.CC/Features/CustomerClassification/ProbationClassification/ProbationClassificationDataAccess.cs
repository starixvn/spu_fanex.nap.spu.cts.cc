namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProbationClassificationDataAccess
    {
        IEnumerable<ProbationEntity> GetProbationCustForScanning(int numberOfRecord);

        IEnumerable<ProbationEntity> ClassifyCategoryForProbationCust(IEnumerable<ProbationEntity> custList);

        void InsertProbationNotification(IEnumerable<ProbationEntity> scanList, IEnumerable<ProbationEntity> failProbations);

        void UpdateLastScanDateForProbationCust(string custSport);
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
            var criteria = new CustClassificationForProbationScanningGetCriteria
            {
                NumberOfRecord = numberOfRecord
            };

            return repository.Fetch<ProbationEntity>(criteria);
        }

        public IEnumerable<ProbationEntity> ClassifyCategoryForProbationCust(IEnumerable<ProbationEntity> custList)
        {
            var criteria = new NormalAccountsClassificationProbationClassifyCriteria
            {
                CustomersXML = custList.ToXmlString()
            };

            return repository.Fetch<ProbationEntity>(criteria);
        }

        public void InsertProbationNotification(IEnumerable<ProbationEntity> scanList, IEnumerable<ProbationEntity> failProbations)
        {
            var criteria = new ProbationNotificationInsertCriteria
            {
                TotalScanned = scanList.Count(),
                TotalFailed = failProbations.Count(),
                TotalGeneralFailed = failProbations.Count(c => c.SportGroupId == 200)
            };

            repository.Execute(criteria);
        }

        public void UpdateLastScanDateForProbationCust(string custSport)
        {
            var criteria = new LastScanDateForProbationCustUpdateCriteria
            {
                CustSportJson = custSport
            };

            repository.Execute(criteria);
        }
    }
}