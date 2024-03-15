namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousScoreClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyDangerousClassificationService
    {
        IEnumerable<DangerousDetectionToCCEntity> GetCustDangerousDetection(DateTime scannedDate);

        IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId);
    }

    public class DailyDangerousClassificationService : IDailyDangerousClassificationService
    {
        private readonly IDailyDangerousClassificationDataAccess dangerousClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISystemParameterServices systemParameterService;

        public DailyDangerousClassificationService(ISystemParameterServices systemParameterService,
            IProblemAccountServices problemAccountServices,
            IDailyDangerousClassificationDataAccess dangerousClassificationDataAccess,
            ICustClassToMainDBService custClassToMainDBService)
        {
            this.systemParameterService = systemParameterService;
            this.problemAccountServices = problemAccountServices;
            this.dangerousClassificationDataAccess = dangerousClassificationDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public IEnumerable<DangerousDetectionToCCEntity> GetCustDangerousDetection(DateTime scannedDate)
        {
            return dangerousClassificationDataAccess.GetDangerousScoreToCC(scannedDate);
        }

        public IEnumerable<ProblemAccountLosingClassificationEntity> GetProblemAccountLosingClassification(IEnumerable<long> custIds, FunctionId funcId)
        {
            return problemAccountServices.GetProblemAccountLosingClassification(custIds, funcId);
        }

        public void CheckIsCCForDangerousScore()
        {

        }
    }
}
