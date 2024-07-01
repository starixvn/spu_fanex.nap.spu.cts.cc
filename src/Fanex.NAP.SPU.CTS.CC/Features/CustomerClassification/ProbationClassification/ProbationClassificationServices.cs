namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

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
        private readonly ISystemParameterDataAccess systemParameterDataAccess;
        private ProbationModel probationModel;

        public ProbationClassificationServices(IProbationClassificationDataAccess probationClassificationDataAccess, ISystemParameterDataAccess systemParameterDataAccess)
        {
            this.probationClassificationDataAccess = probationClassificationDataAccess;
            this.systemParameterDataAccess = systemParameterDataAccess;
        }

        public IProbationClassificationServices Start()
        {
            probationModel = new ProbationModel();

            return this;
        }

        public IProbationClassificationServices GetProbationCustForScanning(int batchSize)
        {
            if (batchSize > 0)
            {
                probationModel = probationClassificationDataAccess.GetProbationCustForScanning(batchSize);
            }

            return this;
        }

        public IProbationClassificationServices ClassifyProbation()
        {
            if (probationModel != null && probationModel.CustProbations != null && probationModel.CustProbations.Any())
            {
                var custToString = string.Join(",", probationModel.CustProbations.Select(c => c.CustId).Distinct());

                probationClassificationDataAccess.ClassifyProbation(custToString);
            }

            return this;
        }

        public bool UpdateLastScanDate()
        {
            //if (probationModel != null && probationModel.CustProbations != null && probationModel.CustProbations.Any())
            //{
            //    var custJson = JsonConvert.SerializeObject(custProbation);

            //    probationClassificationDataAccess.UpdateLastScanDate(custJson);

            //    return true;
            //}

            //return false;

            if (probationModel != null && probationModel.CustProbations != null && probationModel.CustProbations.Any())
            {
                systemParameterDataAccess.UpdateTimingExecution(probationModel.LastCustID.ToString(), CTS_SystemParameters.DailyProbation_LastCustID);
                systemParameterDataAccess.UpdateTimingExecution(probationModel.LastCategoryID.ToString(), CTS_SystemParameters.DailyProbation_LastCategoryID);

                return true;
            }

            return false;
        }

        public IEnumerable<ProbationEntity> Customers => probationModel.CustProbations;
    }
}