namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProbationClassificationBySportService
    {
        IProbationClassificationBySportService Start();

        IProbationClassificationBySportService GetProbationScanClassificationBySport();

        IProbationClassificationBySportService PreprocessProbationClassificationBySport();

        bool UpdateCompleteProbationScanBySport();

        IEnumerable<ProbationScanClassificationBySportEntity> ProbationClassification { get; }

    }

    public class ProbationClassificationBySportService : IProbationClassificationBySportService
    {
        private readonly IProbationClassificationBySportDataAccess dataAccess;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;
        private readonly ISentryClient sentryClient;

        private ProbationBySportModel probationData;
        public IEnumerable<ProbationScanClassificationBySportEntity> ProbationClassification => probationData.CustProbations;

        public ProbationClassificationBySportService(
                IProbationClassificationBySportDataAccess probationClassificationDataAccess
            ,   ISentryClient sentryClient
            ,   ISystemParameterDataAccess systemParameterDataAccess)
        {            
            this.dataAccess = probationClassificationDataAccess;
            this.sentryClient = sentryClient;
            this.systemParameterDataAccess = systemParameterDataAccess;
        }

        public IProbationClassificationBySportService Start()
        {
            this.probationData.CustProbations = new List<ProbationScanClassificationBySportEntity>();
            
            return this;
        }

        public IProbationClassificationBySportService GetProbationScanClassificationBySport()
        {
            this.probationData = dataAccess.GetProbationScanClassificationBySport();            

            return this;
        }

        public IProbationClassificationBySportService PreprocessProbationClassificationBySport()
        {
            if (this.probationData.CustProbations != null && this.probationData.CustProbations.Any())
            {
                dataAccess.PreprocessProbationClassificationBySport(this.probationData.CustProbations);               
            }

            return this;
        }

        public bool UpdateCompleteProbationScanBySport()
        {
            systemParameterDataAccess.UpdateTimingExecution(probationData.LastCustID.ToString(), CTS_SystemParameters.DailyProbation_BySport_LastCategoryID);
            systemParameterDataAccess.UpdateTimingExecution(probationData.LastCategoryID.ToString(), CTS_SystemParameters.DailyNormalClassificationBySportLastCustID);

            return probationData.CustProbations != null && probationData.CustProbations.Any();
        }
    }
}
