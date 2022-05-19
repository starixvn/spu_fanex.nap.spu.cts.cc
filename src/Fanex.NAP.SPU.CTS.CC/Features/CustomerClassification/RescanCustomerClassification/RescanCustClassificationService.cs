namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RobotAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanCustClassificationService
    {
        IRescanCustClassificationService Start();

        bool RescanCustomerClassify(RescanParametersEntity paras);

        IRescanCustClassificationService GetExistedProblemAccounts(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertProblemAccount(string serviceName);

        IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds);

        IRescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertRobot(RescanParametersEntity paras);

        IRescanCustClassificationService PushExternalData(RescanParametersEntity paras);

        IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras);

        IRescanCustClassificationService LogRescanPAData(IEnumerable<RobotInsetViewEntity> RobotUser, FunctionId funcId, string stageName);

        IRescanCustClassificationService GetProbAccountLosingClassification(FunctionId functionId);

        IEnumerable<long> CustIdsToRescanPA { get; }
        
        IEnumerable<RobotInsetViewEntity> RobotUsers { get; }
    }

    public class RescanCustClassificationService : IRescanCustClassificationService
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISentryClient sentryClient;
        private readonly IDailyNormalClassificationServices dailyClassificationServices;
        private readonly IDailyProblemClassificationServices dailyProblemClassificationServices;
        private IEnumerable<ProblemAccountCategoryEntity> problemAccounts;
        private IEnumerable<ProblemAccountLosingClassificationEntity> problemAccountLosing;
        private List<RobotInsetViewEntity> robotUsers;
        private List<long> custIdsToRescanPA;
        private List<long> smaCustIds;
        private bool isSuccess;

        public RescanCustClassificationService(
            IRescanCustClassificationDataAccess rescanCustClassificationDataAccess,
            ISentryClient sentryClient,
            IProblemAccountServices problemAccountServices,
            IAntiFraudCenterServices antiFraudCenterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IDailyNormalClassificationServices dailyClassificationServices,
            IDailyProblemClassificationServices dailyProblemClassificationServices)
        {
            this.rescanCustClassificationDataAccess = rescanCustClassificationDataAccess;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.sentryClient = sentryClient;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.dailyClassificationServices = dailyClassificationServices;
            this.dailyProblemClassificationServices = dailyProblemClassificationServices;
        }

        public IRescanCustClassificationService Start()
        {
            this.problemAccountLosing = Enumerable.Empty<ProblemAccountLosingClassificationEntity>();
            this.problemAccounts = Enumerable.Empty<ProblemAccountCategoryEntity>();
            this.robotUsers = new List<RobotInsetViewEntity>();
            this.custIdsToRescanPA = new List<long>();
            this.isSuccess = true;

            return this;
        }

        public bool RescanCustomerClassify(RescanParametersEntity paras)
        {
            if (paras.CustIds != null && paras.CustIds.Any())
            {
                Start();

                if (paras.IsRescanPA)
                {
                    GetExistedProblemAccounts(paras.CustIds)
                   .GetProbAccountLosingClassification(paras.FunctionId)
                   .InsertProblemAccount(paras.ServiceName)
                   .GetRobotUsers(paras.CustIds).LogRescanPAData(robotUsers, paras.FuncId_CheckRobotUser, paras.ServiceName)
                   .InsertRobot(paras)
                   .PushExternalData(paras);
                }
                
                GetAgentCusts(paras.CustIds)       
               .RescanNormalAccount(paras);
            }

            return isSuccess;
        }

        public IRescanCustClassificationService GetExistedProblemAccounts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                problemAccounts = problemAccountServices.GetExistedProblemAccounts(custIds);
            }

            return this;
        }

        public IRescanCustClassificationService GetProbAccountLosingClassification(FunctionId functionId)
        {
            if (problemAccounts.Any())
            {
                var custIds = problemAccounts.Select(c => c.CustID);

                problemAccountLosing = dailyProblemClassificationServices.GetProblemAccountLosingClassification(custIds, functionId);
            }

            return this;
        }

        public IRescanCustClassificationService InsertProblemAccount(string serviceName)
        {
            if (problemAccounts.Any())
            {
                var result = problemAccountServices.InsertProblemAccount(problemAccounts, problemAccountLosing, serviceName);

                isSuccess = result.ErrorCode == 0;

                if(result.ErrorCode == 0 && result.customerInfo != null && result.customerInfo.Any())
                {
                   var custIds = result.customerInfo.Select(c => c.CustID).Distinct();

                   custIdsToRescanPA.AddRange(custIds);
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
                        robotUsers.Add(new RobotInsetViewEntity
                        {
                            CustID = c.CustId,
                            RobotCounter = c.Counter,
                            ReasonID = (int)Category.Robot,
                            Creator = StarixITId
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
                //TODO

                //if (result.ErrorCode == 0 && result.CustIds != null && result.CustIds.Any())
                //{
                //    custIdsToRescanPA.AddRange(result.CustIds);
                //}
            }

            return this;
        }

        public IRescanCustClassificationService PushExternalData(RescanParametersEntity paras)
        {
            if (custIdsToRescanPA.Any())
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

                    smaCustIds.AddRange(agentIds);
                }
            }

            return this;
        }

        public IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras)
        {
            if (paras.CustIds != null && paras.CustIds.Any())
            {
                var custToRescanNormal = paras.CustIds.ToList();
                custToRescanNormal.RemoveAll(x => custIdsToRescanPA.Contains(x));
                custToRescanNormal.RemoveAll(x => smaCustIds.Contains(x));
                
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

        public IRescanCustClassificationService LogRescanPAData(IEnumerable<RobotInsetViewEntity> robotUsers, FunctionId funcId, string stageName)
        {
            custClassToMainDBService.LogInputData(new LogInputViewEntity { RobotUser = robotUsers }, funcId, stageName);

            return this;
        }

        public IEnumerable<long> CustIdsToRescanPA => custIdsToRescanPA;

        public IEnumerable<RobotInsetViewEntity> RobotUsers => robotUsers;
    }
}