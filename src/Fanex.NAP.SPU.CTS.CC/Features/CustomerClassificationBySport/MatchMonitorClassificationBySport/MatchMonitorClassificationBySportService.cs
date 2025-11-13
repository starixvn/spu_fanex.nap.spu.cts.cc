namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMatchMonitorClassificationBySportService
    {
        ClassifyGBMatchMonitorModel ClassifyMatchMonitor();

        void ClassifyMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel);

        bool InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> pas, PushDataToExternalEntity paras);
    }

    public class MatchMonitorClassificationBySportService : ProblemAccountServices, IMatchMonitorClassificationBySportService
    {
        private readonly IMatchMonitorClassificationBySportDataAccess matchDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        public MatchMonitorClassificationBySportService(
            IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustClassToMainDBService custClassToMainDBService,
            IMatchMonitorClassificationBySportDataAccess matchDataAccess) : base(baseDataAccess, problemAccountDataAccess)
        {
            this.matchDataAccess = matchDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public ClassifyGBMatchMonitorModel ClassifyMatchMonitor()
        {
            return matchDataAccess.ClassifyMatchMonitor();
        }

        public void ClassifyMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel)
        {
            if (classifyGBMMModel.MatchID.HasValue)
            {
                matchDataAccess.ClassifyMatchMonitorCompleted(classifyGBMMModel.MatchID.Value, classifyGBMMModel.MMDetailIDList);
            }
        }

        public bool InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> pas, PushDataToExternalEntity paras)
        {
            var paCustIDs = InsertProblemAccountPackage(pas, FunctionId.ClassifyGBMatchMonitor, CCInputFlow.InsertPACategory, false, paras.ServiceName);

            custClassToMainDBService.PushExternalProblemAcountPackage(paCustIDs.Distinct(), paras);

            return true;
        }
    }
}
