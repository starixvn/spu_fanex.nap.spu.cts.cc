namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMatchMonitorClassificationService
    {
        IEnumerable<int> GetMatchMonitor();

        IEnumerable<MatchesInfosEntity> GetMatchInfosByIDs(string matchIDs);

        void UpdateStatusMatchMonitor(IEnumerable<MatchStatusModel> matchStatuses);

        ClassifyGBMatchMonitorModel ClassifyMatchMonitor();

        void ClassifyMatchMonitorCompleted(ClassifyGBMatchMonitorModel classifyGBMMModel);

        bool InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> pas, PushDataToExternalEntity paras);
    }

    public class MatchMonitorClassificationService : ProblemAccountServices, IMatchMonitorClassificationService
    {
        private readonly IMatchMonitorClassificationDataAccess matchDataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;

        public MatchMonitorClassificationService(IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustClassToMainDBService custClassToMainDBService,
            IMatchMonitorClassificationDataAccess matchDataAccess) : base(baseDataAccess, problemAccountDataAccess)
        {
            this.matchDataAccess = matchDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
        }

        public IEnumerable<int> GetMatchMonitor()
        {
            return matchDataAccess.GetMatchMonitor();
        }

        public IEnumerable<MatchesInfosEntity> GetMatchInfosByIDs(string matchIDs)
        {
            if (!string.IsNullOrEmpty(matchIDs))
            {
                return matchDataAccess.GetMatchInfosByIDs(matchIDs);
            }

            return Enumerable.Empty<MatchesInfosEntity>();
        }

        public void UpdateStatusMatchMonitor(IEnumerable<MatchStatusModel> matchStatuses)
        {
            matchDataAccess.UpdateStatusMatchMonitor(matchStatuses);
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
