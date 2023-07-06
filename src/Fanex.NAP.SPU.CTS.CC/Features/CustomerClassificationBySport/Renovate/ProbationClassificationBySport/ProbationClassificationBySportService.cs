namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.ProbationClassificationBySport
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IProbationClassificationBySportService
    {
        IProbationClassificationBySportService Start();

        IProbationClassificationBySportService GetProbationScanClassificationBySport(int batchSize);

        IProbationClassificationBySportService PreprocessProbationClassificationBySport();

        IEnumerable<ProbationScanClassificationBySportEntity> ProbationClassification { get; }

    }

    public class ProbationClassificationBySportService : IProbationClassificationBySportService
    {
        private readonly IProbationClassificationBySportDataAccess dataAccess;
        
        private IEnumerable<ProbationScanClassificationBySportEntity> probationClassification;
        public IEnumerable<ProbationScanClassificationBySportEntity> ProbationClassification => probationClassification;

        public ProbationClassificationBySportService(IProbationClassificationBySportDataAccess probationClassificationDataAccess)
        {            
            this.dataAccess = probationClassificationDataAccess;
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

       
    }
}
