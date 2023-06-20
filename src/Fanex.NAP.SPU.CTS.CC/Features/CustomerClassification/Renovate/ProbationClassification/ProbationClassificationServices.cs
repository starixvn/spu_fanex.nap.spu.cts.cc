namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.ProbationClassification
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProbationClassificationServices
    {
        IProbationClassificationServices Start();

        IProbationClassificationServices GetProbationCustForScanning(int batchSize);

        IProbationClassificationServices ClassifyProbation();

        bool UpdateLastScanDate();

        public IEnumerable<ProbationEntity> Customers { get; }
    }

    public class ProbationClassificationServices : IProbationClassificationServices
    {
        private readonly IProbationClassificationDataAccess probationClassificationDataAccess;
        private IEnumerable<ProbationEntity> custProbation;

        public ProbationClassificationServices(IProbationClassificationDataAccess probationClassificationDataAccess)
        {
            this.probationClassificationDataAccess = probationClassificationDataAccess;
        }

        public IProbationClassificationServices Start()
        {
            custProbation = new List<ProbationEntity>();

            return this;
        }

        public IProbationClassificationServices GetProbationCustForScanning(int batchSize)
        {
            if (batchSize > 0)
            {
                custProbation = probationClassificationDataAccess.GetProbationCustForScanning(batchSize);
            }

            return this;
        }

        public IProbationClassificationServices ClassifyProbation()
        {
            if (custProbation != null && custProbation.Any())
            {
                var custToString = string.Join(",", custProbation.Select(c => c.CustId).Distinct());

                probationClassificationDataAccess.ClassifyProbation(custToString);
            }

            return this;
        }

        public bool UpdateLastScanDate()
        {
            if (custProbation != null && custProbation.Any())
            {
                var custJson = JsonConvert.SerializeObject(custProbation);

                probationClassificationDataAccess.UpdateLastScanDate(custJson);

                return true;
            }

            return false;
        }

        public IEnumerable<ProbationEntity> Customers => custProbation;
    }
}