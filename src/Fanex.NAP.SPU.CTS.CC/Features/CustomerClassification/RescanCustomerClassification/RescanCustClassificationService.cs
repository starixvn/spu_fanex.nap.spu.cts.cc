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
        IRescanCustClassificationService Start();

        void RescanCustomerClassify(RescanParametersEntity paras);

        IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds);

        IRescanCustClassificationService GetExistedCustEvidences(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertCustEvidenceCategorToCTS(RescanParametersEntity paras);

        IRescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds);

        IEnumerable<long> SyncRobotUserToUpdateCategory(IEnumerable<long> custIds);

        IRescanCustClassificationService GetProblemAccountFromNAPPA(IEnumerable<long> custIds, bool isCheckPAFromNAP);

        IRescanCustClassificationService InsertProblemAccountToCTS(RescanParametersEntity paras);

        IRescanCustClassificationService RescanProblemAccount(RescanParametersEntity paras);

        IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras);

        IRescanCustClassificationService LogRescanPAData(IEnumerable<ProblemAccountsEntity> problemAccounts, FunctionId funcId, string stageName);

        IEnumerable<long> CustIdsToRescanPA { get; }

        IEnumerable<ProblemAccountsEntity> ProblemAccounts { get; }

        IEnumerable<ProblemAccountsEntity> RobotUsers { get; }

        IEnumerable<CustEvidenceEntity> ExistedEvidences { get; }
    }

    public class RescanCustClassificationService : IRescanCustClassificationService
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;
        private readonly IDailyClassificationServices dailyClassificationServices;
        private List<ProblemAccountsEntity> probAccountNAPPA;
        private List<ProblemAccountsEntity> robotUsers;
        private List<CustEvidenceEntity> existedEvidences;
        private List<long> custIdsToRescanPA;

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
        }

        public IRescanCustClassificationService Start()
        {
            this.custIdsToRescanPA = new List<long>();
            this.probAccountNAPPA = new List<ProblemAccountsEntity>();
            this.robotUsers = new List<ProblemAccountsEntity>();
            this.existedEvidences = new List<CustEvidenceEntity>();

            return this;
        }

        public void RescanCustomerClassify(RescanParametersEntity paras)
        {
            if (paras.CustIds != null && paras.CustIds.Any())
            {
                Start()
               .GetAgentCusts(paras.CustIds)
               .GetExistedCustEvidences(paras.CustIds).InsertCustEvidenceCategorToCTS(paras)
               .GetRobotUsers(paras.CustIds).LogRescanPAData(robotUsers, paras.FuncId_CheckRobotUser, paras.ServiceName)
               .GetProblemAccountFromNAPPA(paras.CustIds, paras.IsCheckPAFromNAP).LogRescanPAData(probAccountNAPPA, paras.FuncId_CheckPA, paras.ServiceName)
               .InsertProblemAccountToCTS(paras)
               .RescanProblemAccount(paras)
               .RescanNormalAccount(paras);
            }
        }

        public IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds)
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

        public IRescanCustClassificationService GetExistedCustEvidences(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                string custIdsToString = string.Join(",", custIds);

                var accountEvidences = rescanCustClassificationDataAccess.GetExistedEvidences(custIdsToString);

                existedEvidences.AddRange(accountEvidences);
            }

            return this;
        }

        public IRescanCustClassificationService InsertCustEvidenceCategorToCTS(RescanParametersEntity paras)
        {
            var isWriteLogRescan = paras.FunctionId != FunctionId.InsertNewCustJob;

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

        public IRescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var robots = SyncRobotUserToUpdateCategory(custIds);

                if (robots != null && robots.Any())
                {
                    robots.ToList().ForEach(c =>
                    {
                        robotUsers.Add(new ProblemAccountsEntity
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

        public IRescanCustClassificationService GetProblemAccountFromNAPPA(IEnumerable<long> custIds, bool isCheckPAFromNAP)
        {
            if (custIds != null && custIds.Any() && isCheckPAFromNAP)
            {
                string custIdsToString = string.Join(",", custIds);

                var problemAccountsNAPCM = problemAccountServices.GetProblemAccounts_IBC(custIdsToString, null);
                var problemAccountsCN88 = problemAccountServices.GetProblemAccounts_CN88(custIdsToString, null);

                probAccountNAPPA.AddRange(problemAccountsNAPCM);
                probAccountNAPPA.AddRange(problemAccountsCN88);
            }

            return this;
        }

        public IRescanCustClassificationService InsertProblemAccountToCTS(RescanParametersEntity paras)
        {
            var insertProblemAccounts = new List<ProblemAccountsEntity>();

            if (robotUsers != null && robotUsers.Any())
            {
                insertProblemAccounts.AddRange(robotUsers);
            }

            if (probAccountNAPPA != null && probAccountNAPPA.Any())
            {
                insertProblemAccounts.AddRange(probAccountNAPPA);
            }

            if (insertProblemAccounts.Any())
            {
                var problemAccountsChunk = insertProblemAccounts.Distinct().Chunk(paras.InternalBatchSize);

                var isWriteLogRescan = paras.FunctionId != FunctionId.InsertNewCustJob;

                foreach (var problemAccs in problemAccountsChunk)
                {
                    var problemCustIds = problemAccountServices.InsertProblemAccountToCTS(problemAccs, isWriteLogRescan, paras.ServiceName, paras.IsWriteLog);

                    custIdsToRescanPA.AddRange(problemCustIds.CustIds);
                }
            }

            return this;
        }

        public IRescanCustClassificationService RescanProblemAccount(RescanParametersEntity paras)
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

        public IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras)
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

        public IRescanCustClassificationService LogRescanPAData(IEnumerable<ProblemAccountsEntity> problemAccounts, FunctionId funcId, string stageName)
        {
            custClassToMainDBService.LogInputData(new LogInputViewEntity { ProblemAccounts = problemAccounts }, funcId, stageName);

            return this;
        }

        public IEnumerable<long> CustIdsToRescanPA => custIdsToRescanPA;

        public IEnumerable<ProblemAccountsEntity> ProblemAccounts => probAccountNAPPA;

        public IEnumerable<ProblemAccountsEntity> RobotUsers => robotUsers;

        public IEnumerable<CustEvidenceEntity> ExistedEvidences => existedEvidences;
    }
}