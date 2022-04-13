namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence;
    using Fanex.NAP.SPU.CTS.CC.Features.Evidence.CustomerEvidenceInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanCustClassificationService
    {
        IRescanCustClassificationService Start();

        bool RescanCustomerClassify(RescanParametersEntity paras);

        IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds);

        IRescanCustClassificationService GetExistedCustEvidences(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertCustEvidenceCategory(RescanParametersEntity paras);

        IRescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds);

        IRescanCustClassificationService GetProblemAccountFromNAP(IEnumerable<long> custIds, bool isCheckPAFromNAP);

        IRescanCustClassificationService InsertRobot(RescanParametersEntity paras);

        IRescanCustClassificationService InsertProblemAccount(RescanParametersEntity paras);

        IRescanCustClassificationService RescanProblemAccount(RescanParametersEntity paras);

        IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras);

        IRescanCustClassificationService LogRescanPAData(IEnumerable<ProblemAccountsEntity> problemAccounts, FunctionId funcId, string stageName);

        IRescanCustClassificationService GetProbAccountLosingClassification(FunctionId functionId);

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
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;
        private readonly IDailyNormalClassificationServices dailyClassificationServices;
        private readonly ICustomerEvidenceInsertServices customerEvidenceInsertServices;
        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private List<long> custIdsToRescanPA;
        private IEnumerable<CustEvidenceEntity> existedEvidences;
        private IEnumerable<ProblemAccountsEntity> problemAccountFromNAP;
        private List<ProblemAccountsEntity> robotUsers;
        private bool isSuccess;
        private List<ProblemAccountLosingClassificationEntity> problemAccountLosing;
        private const string ServiceNameRescan = "Rescan Customer Classification";

        public RescanCustClassificationService(
            IRescanCustClassificationDataAccess rescanCustClassificationDataAccess,
            ISentryClient sentryClient,
            IProblemAccountServices problemAccountServices,
            IAntiFraudCenterServices antiFraudCenterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IDailyNormalClassificationServices dailyClassificationServices,
            ICustomerEvidenceInsertServices customerEvidenceInsertServices,
            IDailyProblemClassificationServices dailyProblemClassificationServices)
        {
            this.rescanCustClassificationDataAccess = rescanCustClassificationDataAccess;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.sentryClient = sentryClient;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.dailyClassificationServices = dailyClassificationServices;
            this.customerEvidenceInsertServices = customerEvidenceInsertServices;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
        }

        public IRescanCustClassificationService Start()
        {
            this.custIdsToRescanPA = new List<long>();
            this.existedEvidences = Enumerable.Empty<CustEvidenceEntity>();
            this.problemAccountFromNAP = Enumerable.Empty<ProblemAccountsEntity>();
            this.robotUsers = new List<ProblemAccountsEntity>();
            this.problemAccountLosing = new List<ProblemAccountLosingClassificationEntity>();
            this.isSuccess = true;

            return this;
        }

        public bool RescanCustomerClassify(RescanParametersEntity paras)
        {
            if (paras.CustIds != null && paras.CustIds.Any())
            {
                Start()
               .GetAgentCusts(paras.CustIds)
               .GetExistedCustEvidences(paras.CustIds)
               .GetProblemAccountFromNAP(paras.CustIds, paras.IsCheckPAFromNAP).LogRescanPAData(problemAccountFromNAP, paras.FuncId_CheckPA, paras.ServiceName)
               .GetProbAccountLosingClassification(paras.FunctionId)
               .InsertCustEvidenceCategory(paras)
               .InsertProblemAccount(paras)
               .GetRobotUsers(paras.CustIds).LogRescanPAData(robotUsers, paras.FuncId_CheckRobotUser, paras.ServiceName)
               .InsertRobot(paras)
               .RescanProblemAccount(paras)
               .RescanNormalAccount(paras);
            }

            return isSuccess;
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

                existedEvidences = rescanCustClassificationDataAccess.GetExistedEvidences(custIdsToString);
            }

            return this;
        }

        public IRescanCustClassificationService InsertCustEvidenceCategory(RescanParametersEntity paras)
        {
            var isRescan = paras.FunctionId != FunctionId.InsertNewCustJob;

            if (existedEvidences != null && existedEvidences.Any())
            {
                var evidencesChunk = existedEvidences.Chunk(paras.InternalBatchSize);

                foreach (var evidences in evidencesChunk)
                {
                    var result = customerEvidenceInsertServices.SyncCustomerEvidenceToCTSCategory(evidences, problemAccountLosing, isRescan);

                    if (result.ErrorCode == -1)
                    {
                        Utilities.LogSentryError(paras.ServiceName, "RescanCC - SyncCustEvidence fail", evidences, new Exception(result.ErrorMsg), sentryClient);
                    }
                    else
                    {
                        if (result.CustInfo != null && result.CustInfo.Any())
                        {
                            var custIds = result.CustInfo.Select(x => x.CustID);

                            custIdsToRescanPA.AddRange(custIds);
                        }
                    }
                }
            }

            return this;
        }

        public IRescanCustClassificationService GetProblemAccountFromNAP(IEnumerable<long> custIds, bool isCheckPAFromNAP)
        {
            if (custIds != null && custIds.Any() && isCheckPAFromNAP)
            {
                string custIdsToString = string.Join(",", custIds);

                problemAccountFromNAP = problemAccountServices.GetProblemAccounts_IBC(custIdsToString, null);
            }

            return this;
        }

        public IRescanCustClassificationService GetProbAccountLosingClassification(FunctionId functionId)
        {
            var custIds = new List<long>();

            if (existedEvidences != null && existedEvidences.Any())
            {
                custIds.AddRange(existedEvidences.Select(c => c.CustId));
            }

            if (problemAccountFromNAP != null && problemAccountFromNAP.Any())
            {
                custIds.AddRange(problemAccountFromNAP.Select(c => c.CustID));
            }

            if (custIds.Any())
            {
                var custIdChunk = custIds.Distinct().Chunk(ProblemAccountLosing_BatchSize);

                foreach (var custIdBatch in custIdChunk)
                {
                    var paLosing = dailyProblemClassificationServices.GetProblemAccountLosingClassification(custIdBatch, functionId);

                    if (paLosing != null)
                    {
                        problemAccountLosing.AddRange(paLosing);
                    }
                }
            }

            return this;
        }

        public IRescanCustClassificationService InsertProblemAccount(RescanParametersEntity paras)
        {
            if (problemAccountFromNAP != null && problemAccountFromNAP.Any())
            {
                var problemAccountsChunk = problemAccountFromNAP.Distinct().Chunk(paras.InternalBatchSize);

                var isRescanFlow = paras.FunctionId != FunctionId.InsertNewCustJob;

                foreach (var problemAccs in problemAccountsChunk)
                {
                    var problemCustIds = problemAccountServices.InsertProblemAccount(problemAccs, problemAccountLosing, isRescanFlow, paras.ServiceName);

                    custIdsToRescanPA.AddRange(problemCustIds.CustIds);
                }
            }

            return this;
        }

        public IRescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var robots = rescanCustClassificationDataAccess.GetExistedRobotUser(string.Join(",", custIds));

                if (robots != null && robots.Any())
                {
                    robots.ToList().ForEach(c =>
                    {
                        robotUsers.Add(new ProblemAccountsEntity
                        {
                            CustID = c.CustId,
                            RobotCounter = c.Counter,
                            ReasonID = (int)Category.Robot
                        });
                    });
                }
            }

            return this;
        }

        public IRescanCustClassificationService InsertRobot(RescanParametersEntity paras)
        {
            if (robotUsers != null && robotUsers.Any())
            {
                var robotChunk = robotUsers.Distinct().Chunk(paras.InternalBatchSize);

                var isRescanFlow = paras.FunctionId != FunctionId.InsertNewCustJob;

                foreach (var robotBatch in robotChunk)
                {
                    var problemCustIds = problemAccountServices.InsertProblemAccount(robotBatch, null, isRescanFlow, paras.ServiceName);

                    custIdsToRescanPA.AddRange(problemCustIds.CustIds);
                }
            }

            return this;
        }

        public IRescanCustClassificationService RescanProblemAccount(RescanParametersEntity paras)
        {
            if (custIdsToRescanPA != null && custIdsToRescanPA.Any())
            {
                Parallel.Invoke(() =>
                {
                    isSuccess = custClassToMainDBService.PushDataToExternal(custIdsToRescanPA.Distinct(), new PushDataToExternalEntity
                    {
                        ExternalBatchSize = paras.ExternalBatchSize,
                        FeedSoon88API = paras.FeedSoon88API,
                        IsPushCCToSoon88 = paras.IsPushCCToSoon88,
                        IsUpdateDangerToMainDB = paras.IsUpdateDangerToMainDB,
                        DangerLevelType = paras.DangerLevelType,
                        IsWriteLog = paras.IsWriteLog,
                        ServiceName = paras.ServiceName
                    });
                },
                () =>
                {
                    antiFraudCenterServices.InsertAFCAccounts(custIdsToRescanPA.Distinct(), FromFlow.CTSServices, paras.IsSyncAFCData, paras.AFCInsertAccountURL);
                });
            }

            return this;
        }

        public IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras)
        {
            if (paras.CustIds != null && paras.CustIds.Any())
            {
                var custToRescanNormal = paras.CustIds.ToList();
                custToRescanNormal.RemoveAll(x => custIdsToRescanPA.Contains(x));

                if (custToRescanNormal.Any())
                {
                    var custIdsChunk = custToRescanNormal.Chunk(paras.ExternalBatchSize);

                    foreach (var custIds in custIdsChunk)
                    {
                        dailyClassificationServices.ScanningDailyNormalClassification(custIds, paras.FunctionId, false);
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

        public IEnumerable<ProblemAccountsEntity> ProblemAccounts => problemAccountFromNAP;

        public IEnumerable<ProblemAccountsEntity> RobotUsers => robotUsers;

        public IEnumerable<CustEvidenceEntity> ExistedEvidences => existedEvidences;
    }
}