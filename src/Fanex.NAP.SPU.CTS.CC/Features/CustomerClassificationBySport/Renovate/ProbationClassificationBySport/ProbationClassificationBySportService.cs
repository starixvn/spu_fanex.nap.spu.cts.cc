namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport
{
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProbationClassificationBySportService
    {
        IProbationClassificationBySportService Start();

        IProbationClassificationBySportService GetProbationScanClassificationBySport(int batchSize);

        IProbationClassificationBySportService PreprocessProbationClassificationBySport();

        void UpdateScanDateProbationClassificationBySport();

        IEnumerable<ProbationScanClassificationBySportEntity> ProbationClassification { get; }

    }

    public class ProbationClassificationBySportService : IProbationClassificationBySportService
    {
        private readonly IProbationClassificationBySportDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        private IEnumerable<ProbationScanClassificationBySportEntity> probationClassification;
        public IEnumerable<ProbationScanClassificationBySportEntity> ProbationClassification => probationClassification;

        public ProbationClassificationBySportService(IProbationClassificationBySportDataAccess probationClassificationDataAccess
            , ISentryClient sentryClient)
        {            
            this.dataAccess = probationClassificationDataAccess;
            this.sentryClient = sentryClient;
        }

        public IProbationClassificationBySportService Start()
        {
            this.probationClassification = new List<ProbationScanClassificationBySportEntity>();
            
            return this;
        }

        public IProbationClassificationBySportService GetProbationScanClassificationBySport(int batchSize)
        {
            this.probationClassification = dataAccess.GetProbationScanClassificationBySport(batchSize);            

            return this;
        }

        public IProbationClassificationBySportService PreprocessProbationClassificationBySport()
        {
            if (this.probationClassification != null && this.probationClassification.Any())
            {
                dataAccess.PreprocessProbationClassificationBySport(this.probationClassification);               
            }

            return this;
        }

        public void UpdateScanDateProbationClassificationBySport()
        {
            if (this.probationClassification != null && this.probationClassification.Any())
            {
                var custJson = JsonConvert.SerializeObject(this.probationClassification.Select(x => new CustSportModel
                {
                    CustID = x.CustId,
                    SportID = x.SportGroup
                }));

                dataAccess.UpdateScanDateProbationClassificationBySport(custJson);               
            }
        }
    }
}
