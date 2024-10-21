namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
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

        private ProbationBySportModel probationData;

        public IEnumerable<ProbationScanClassificationBySportEntity> ProbationClassification => probationData.CustProbations;

        public ProbationClassificationBySportService(
                IProbationClassificationBySportDataAccess probationClassificationDataAccess
            ,   ISystemParameterDataAccess systemParameterDataAccess)
        {            
            this.dataAccess = probationClassificationDataAccess;
            this.systemParameterDataAccess = systemParameterDataAccess;
        }

        public IProbationClassificationBySportService Start()
        {
            this.probationData = new ProbationBySportModel();
            
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
            systemParameterDataAccess.UpdateTimingExecutionForCTS(probationData.LastCustID.ToString(), CTS_SystemParameters.DailyProbation_BySport_LastCustID);

            return probationData.CustProbations != null && probationData.CustProbations.Any();
        }
    }
}
