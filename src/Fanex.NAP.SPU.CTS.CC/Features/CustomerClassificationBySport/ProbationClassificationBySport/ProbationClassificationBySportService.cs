namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{    
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;   

    public interface IProbationClassificationBySportService
    {
        IProbationClassificationBySportService Start();

        IProbationClassificationBySportService GetProbationCustClassificationBySport(int batchSize);

        IProbationClassificationBySportService ScanProbationClassificationBySport();

        void UpdateScanDateProbationClassificationBySport();

        IEnumerable<ProbationClassificationBySportEntity> CustomerClassification { get; }
    }

    public class ProbationClassificationBySportService : IProbationClassificationBySportService
    {
        private readonly IProbationClassificationBySportDataAccess probationClassificationDataAccess;
        private IEnumerable<ProbationClassificationBySportEntity> probationClassification;

        public ProbationClassificationBySportService(IProbationClassificationBySportDataAccess probationClassificationDataAccess)
        {
            this.probationClassificationDataAccess = probationClassificationDataAccess;
        }        

        public IProbationClassificationBySportService Start()
        {
            probationClassification = new List<ProbationClassificationBySportEntity>();

            return this;
        }

        public IProbationClassificationBySportService GetProbationCustClassificationBySport(int batchSize)
        {
            if (batchSize > 0)
            {
                probationClassification = probationClassificationDataAccess.GetProbationCustClassificationBySport(batchSize);
            }

            return this;
        }

        public IProbationClassificationBySportService ScanProbationClassificationBySport()
        {
            if (this.probationClassification != null && this.probationClassification.Any())
            {
                var probations = this.probationClassification.Select(x => new ScanProbationClassificationBySportEntity
                {
                    CustId = x.CustID,
                    SportId = x.SportID,
                    IsProbationLastDay = x.IsProbationLastDay
                });

                probationClassificationDataAccess.ScanProbationClassificationBySport(probations);               
            }

            return this;          
        }

        public void UpdateScanDateProbationClassificationBySport()
        {
            if (probationClassification != null && probationClassification.Any())
            {
                var custJson = JsonConvert.SerializeObject(probationClassification);

                probationClassificationDataAccess.UpdateScanDateProbationClassificationBySport(custJson);
            }
        }

        public IEnumerable<ProbationClassificationBySportEntity> CustomerClassification => probationClassification;
    }
}
