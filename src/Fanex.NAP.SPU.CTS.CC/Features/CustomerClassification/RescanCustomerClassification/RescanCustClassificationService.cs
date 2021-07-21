namespace Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.DailyClassification;
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanCustClassificationService
    {
        void RescanCustomerClassify(RescanParametersEntity paras);

        RescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds);

        RescanCustClassificationService GetExistedCustEvidences(IEnumerable<long> custIds);

        RescanCustClassificationService InsertCustEvidenceCategorToCTS(RescanParametersEntity paras);

        RescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds);

        IEnumerable<long> SyncRobotUserToUpdateCategory(IEnumerable<long> custIds);

        RescanCustClassificationService GetProblemAccountFromNAPPA(IEnumerable<long> custIds, bool isCheckPAFromNAP);

        RescanCustClassificationService InsertProblemAccountToCTS(RescanParametersEntity paras);

        RescanCustClassificationService RescanProblemAccount(RescanParametersEntity paras);

        RescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras);
    }

    public class RescanCustClassificationService : IRescanCustClassificationService
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;
        private readonly IDailyClassificationServices dailyClassificationServices;
        private readonly List<ProblemAccountsEntity> problemAccounts;
        private readonly List<CustEvidenceEntity> existedEvidences;
        private readonly List<long> custIdsToRescanPA;

        public RescanCustClassificationService(
            IRescanCustClassificationDataAccess rescanCustClassificationDataAccess,
            ISentryClient sentryClient,
            IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService,
            IDailyClassificationServices dailyClassificationServices)
        {
            this.rescanCustClassificationDataAccess = rescanCustClassificationDataAccess;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.sentryClient = sentryClient;
            this.dailyClassificationServices = dailyClassificationServices;
            this.custIdsToRescanPA = new List<long>();
            this.problemAccounts = new List<ProblemAccountsEntity>();
            this.existedEvidences = new List<CustEvidenceEntity>();
        }

        public void RescanCustomerClassify(RescanParametersEntity paras)
        {
            if (paras.CustIds != null && paras.CustIds.Any())
            {
                GetAgentCusts(paras.CustIds)
               .GetExistedCustEvidences(paras.CustIds).InsertCustEvidenceCategorToCTS(paras)
               .GetRobotUsers(paras.CustIds).GetProblemAccountFromNAPPA(paras.CustIds, paras.IsCheckPAFromNAP).InsertProblemAccountToCTS(paras)
               .RescanProblemAccount(paras)
               .RescanNormalAccount(paras);
            }
        }

        public RescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var data = rescanCustClassificationDataAccess.GetRoleCustomer(string.Join(",", custIds));

                if (data != null && data.Any())
                {
                    var agentIds = data.Where(c => c.RoleID == (int)CustomerRole.Agent
                                            || c.RoleID == (int)CustomerRole.Master
                                            || c.RoleID == (int)CustomerRole.Super)
                               .Select(c => c.CustID)?.ToList();

                    custIdsToRescanPA.AddRange(agentIds);
                }
            }

            return this;
        }

        public RescanCustClassificationService GetExistedCustEvidences(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                string custIdsToString = string.Join(",", custIds);

                var accountEvidences = rescanCustClassificationDataAccess.GetExistedEvidences(custIdsToString);

                existedEvidences.AddRange(accountEvidences);
            }

            return this;
        }

        public RescanCustClassificationService InsertCustEvidenceCategorToCTS(RescanParametersEntity paras)
        {
            var isWriteLogRescan = paras.FunctionId != FunctionId.InsertNewCust;

            if (existedEvidences != null && existedEvidences.Any())
            {
                var evidencesChunk = existedEvidences.Chunk(paras.InternalBatchSize);

                foreach (var evidences in evidencesChunk)
                {
                    var result = rescanCustClassificationDataAccess.SyncCustomerEvidenceToCategory(JsonConvert.SerializeObject(evidences), isWriteLogRescan);
                    custIdsToRescanPA.AddRange((IEnumerable<long>)result.OutParam["custIds"]);

                    if (result.ErrorCode == -1)
                    {
                        Utilities.LogSentryError(paras.ServiceName, "RescanCC - SyncCustEvidence fail", evidences, new Exception(result.ErrorMsg), sentryClient);
                    }
                }
            }

            return this;
        }

        public RescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var robotUsers = SyncRobotUserToUpdateCategory(custIds);

                if (robotUsers != null && robotUsers.Any())
                {
                    robotUsers.ToList().ForEach(c =>
                    {
                        problemAccounts.Add(new ProblemAccountsEntity
                        {
                            CustID = c,
                            ReasonID = (int)Category.Robot
                        });
                    });
                }
            }

            return this;
        }

        public IEnumerable<long> SyncRobotUserToUpdateCategory(IEnumerable<long> custIds)
        {
            IEnumerable<long> robotUser = new List<long>();

            var custs = rescanCustClassificationDataAccess.GetCustByRobotSiteRule(string.Join(",", custIds));

            var getRobotUser = rescanCustClassificationDataAccess.GetExistedRobotUser(string.Join(",", custs));

            if (!string.IsNullOrEmpty(getRobotUser))
            {
                robotUser = getRobotUser.Split(',').Select(long.Parse);
            }

            return robotUser;
        }

        public RescanCustClassificationService GetProblemAccountFromNAPPA(IEnumerable<long> custIds, bool isCheckPAFromNAP)
        {
            if (custIds != null && custIds.Any() && isCheckPAFromNAP)
            {
                string custIdsToString = string.Join(",", custIds);

                var problemAccountsNAPCM = problemAccountServices.GetProblemAccounts_IBC(custIdsToString, null);
                var problemAccountsCN88 = problemAccountServices.GetProblemAccounts_CN88(custIdsToString, null);

                problemAccounts.AddRange(problemAccountsNAPCM);
                problemAccounts.AddRange(problemAccountsCN88);
            }

            return this;
        }

        public RescanCustClassificationService InsertProblemAccountToCTS(RescanParametersEntity paras)
        {
            if (problemAccounts != null && problemAccounts.Any())
            {
                var problemAccountsChunk = problemAccounts.Distinct().Chunk(paras.InternalBatchSize);

                var isWriteLogRescan = paras.FunctionId != FunctionId.InsertNewCust;

                foreach (var problemAccs in problemAccountsChunk)
                {
                    var problemCustIds = problemAccountServices.InsertProblemAccountToCTS(problemAccs, isWriteLogRescan, paras.ServiceName, paras.IsWriteLog);

                    custIdsToRescanPA.AddRange(problemCustIds);
                }
            }

            return this;
        }

        public RescanCustClassificationService RescanProblemAccount(RescanParametersEntity paras)
        {
            if (custIdsToRescanPA != null && custIdsToRescanPA.Any())
            {
                custClassToMainDBService.PushDataToExternal(custIdsToRescanPA.Distinct(), new PushDataToExternalEntity
                {
                    ExternalBatchSize = paras.ExternalBatchSize,
                    FeedSoon88API = paras.FeedSoon88API,
                    IsPushCCToSoon88 = paras.IsPushCCToSoon88,
                    IsUpdateDangerToMainDB = false,
                    IsWriteLog = paras.IsWriteLog,
                    ServiceName = paras.ServiceName
                });
            }

            return this;
        }

        public RescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras)
        {
            if (paras.CustIds != null && paras.CustIds.Any())
            {
                paras.CustIds.ToList().RemoveAll(x => custIdsToRescanPA.Contains(x));

                if (paras.CustIds != null && paras.CustIds.Any())
                {
                    var custIdsChunk = paras.CustIds.Chunk(paras.InternalBatchSize);

                    foreach (var custIds in custIdsChunk)
                    {
                        dailyClassificationServices.ClassifyCustomerGeneralCategoryDaily(custIds, paras.FunctionId, false);
                    }
                }
            }

            return this;
        }
    }
}