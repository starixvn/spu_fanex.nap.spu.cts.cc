namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;    
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;

    public interface IRescanCustClassificationService
    {
        bool RescanCustomerClassification(IEnumerable<long> custIds, RescanParametersEntity paras);             

        bool PushExternalProblemAccounts(IEnumerable<long> custIds, PushDataToExternalEntity paras);       
    }

    public class RescanCustClassificationService : ProblemAccountServices, IRescanCustClassificationService
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
      
        private readonly ICustClassToMainDBService custClassToMainDBService;

        public RescanCustClassificationService(IBaseCustomerClassificationDataAccess baseDataAccess,
            IRescanCustClassificationDataAccess rescanCustClassificationDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustClassToMainDBService custClassToMainDBService, ISentryClient sentryClient) : base(baseDataAccess, problemAccountDataAccess, sentryClient)
        {
            this.rescanCustClassificationDataAccess = rescanCustClassificationDataAccess;
            this.custClassToMainDBService = custClassToMainDBService;
        }
        public bool RescanCustomerClassification(IEnumerable<long> custIds, RescanParametersEntity paras)
        {
            var custRescans = GetRescanCustomerClassification(string.Join(",", custIds));

            if (custRescans.CustInfos != null && custRescans.CustInfos.Any())
            {
                var groupingCustRescans = custRescans.CustInfos.GroupBy(x => x.InputFlowID).Select(g => new { InputFlowID = g.Key, CustRescans = g.Select(x => x) });

                foreach (var group in groupingCustRescans)
                {
                    if ((CCInputFlow)group.InputFlowID == CCInputFlow.RescanNormal)
                    {
                        PreNormalClassification(group.CustRescans.Select(x => x.CustID), paras);
                    }
                    else
                    {
                        RescanProblemAccount(group.CustRescans, paras, (CCInputFlow)group.InputFlowID);
                    }
                }               
            }

            return true;
        }

        private void RescanProblemAccount(IEnumerable<GetRescanCustomerClassificationEntity> custNormals, RescanParametersEntity paras, CCInputFlow ccInputFlow)
        {
            var resultCustRescans = base.RescanCustomerClassification(custNormals, paras.FunctionId, ccInputFlow, paras.IsToQueue, paras.ServiceName);

            PushExternalProblemAccounts(resultCustRescans, paras.PushDataExternal);
        }

        private void PreNormalClassification(IEnumerable<long> custNormals, RescanParametersEntity paras)
        {
            var custIDToString = string.Join(",", custNormals);

            rescanCustClassificationDataAccess.PreNormalClassification(custIDToString, paras.FunctionId, paras.RescanNormalType);
        }
    
        public bool PushExternalProblemAccounts(IEnumerable<long> custIds, PushDataToExternalEntity paras)
        {
            if (custIds.Any())
            {
                return custClassToMainDBService.PushExternalProblemAcountPackage(custIds.Distinct(), paras);
            }

            return true;
        }
    }
}