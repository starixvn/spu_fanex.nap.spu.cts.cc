namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProbationClassification
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProbationClassificationServices
    {
        ProbationClassificationServices GetProbationCustForScanning(int batchSize);

        ProbationClassificationServices ScanningProbationClassification();

        void InsertProbationNotification(IEnumerable<ProbationEntity> scanList, IEnumerable<ProbationEntity> failProbations);

        IEnumerable<ProbationEntity> ClassifyCategoryForProbationCust(IEnumerable<ProbationEntity> custList);

        void UpdateLastScanDateForProbationCust();

        IEnumerable<ProbationEntity> CustomerClassification { get; }
    }

    public class ProbationClassificationServices : IProbationClassificationServices
    {
        private readonly IProbationClassificationDataAccess probationClassificationDataAccess;
        private IEnumerable<ProbationEntity> probationClassification;

        public ProbationClassificationServices(IProbationClassificationDataAccess probationClassificationDataAccess)
        {
            this.probationClassificationDataAccess = probationClassificationDataAccess;
            probationClassification = new List<ProbationEntity>();
        }

        public ProbationClassificationServices GetProbationCustForScanning(int batchSize)
        {
            if (batchSize > 0)
            {
                probationClassification = probationClassificationDataAccess.GetProbationCustForScanning(batchSize);
            }

            return this;
        }

        public IEnumerable<ProbationEntity> ClassifyCategoryForProbationCust(IEnumerable<ProbationEntity> custList)
        {
            return probationClassificationDataAccess.ClassifyCategoryForProbationCust(custList);
        }

        public void InsertProbationNotification(IEnumerable<ProbationEntity> scanList, IEnumerable<ProbationEntity> failProbations)
        {
            if (failProbations != null && failProbations.Any())
            {
                probationClassificationDataAccess.InsertProbationNotification(scanList, failProbations);
            }
        }

        public ProbationClassificationServices ScanningProbationClassification()
        {
            if (probationClassification != null && probationClassification.Any())
            {
                var custs = ClassifyCategoryForProbationCust(probationClassification);

                if (custs != null && custs.Any())
                {
                    var failedCusts = custs.Where(cus => !cus.IsPassed);
                    InsertProbationNotification(custs, failedCusts);
                }
            }

            return this;
        }

        public void UpdateLastScanDateForProbationCust()
        {
            if (probationClassification != null && probationClassification.Any())
            {
                var custJson = JsonConvert.SerializeObject(probationClassification);

                probationClassificationDataAccess.UpdateLastScanDateForProbationCust(custJson);
            }
        }

        public IEnumerable<ProbationEntity> CustomerClassification => probationClassification;
    }
}