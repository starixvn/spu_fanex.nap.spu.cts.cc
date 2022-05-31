namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
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

        bool RescanCustomerClassification(RescanParametersEntity paras);

        IRescanCustClassificationService GetExistedProblemAccounts(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertProblemAccountLogics(string serviceName, bool isRescanPAFromQueue);

        IRescanCustClassificationService RemoveOldProblemAccount();

        IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds);

        IRescanCustClassificationService GetTWRobotUsers(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertRobot(RescanParametersEntity paras);

        IRescanCustClassificationService PushExternalData(RescanParametersEntity paras);

        IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras);

        IRescanCustClassificationService LogRescanPAData(IEnumerable<InsertRobotEntity> robotUsers, FunctionId funcId, string stageName);

        IRescanCustClassificationService GetProbAccountLosingClassification(FunctionId functionId);

        IEnumerable<long> CustIdsToRescanPA { get; }

        IEnumerable<InsertRobotEntity> RobotUsers { get; }
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
        private IEnumerable<InsertProblemAccountEntity> problemAccounts;
        private IEnumerable<ProblemAccountLosingClassificationEntity> problemAccountLosing;
        private IEnumerable<InsertRobotEntity> robotClassificationPerformance;
        private List<InsertRobotEntity> robotUsers;
        private List<long> custIdsToRescanPA;
        private IEnumerable<long> custIdsToRemoveOldPA;
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
            this.problemAccounts = Enumerable.Empty<InsertProblemAccountEntity>();
            this.robotUsers = new List<InsertRobotEntity>();
            this.robotClassificationPerformance = Enumerable.Empty<InsertRobotEntity>();
            this.custIdsToRescanPA = new List<long>();
            this.custIdsToRemoveOldPA = Enumerable.Empty<long>();
            this.smaCustIds = new List<long>();
            this.isSuccess = true;

            return this;
        }

        public bool RescanCustomerClassification(RescanParametersEntity parameters)
        {
            if (parameters.CustIds != null && parameters.CustIds.Any())
            {
                Start();

                if (parameters.IsRescanPA)
                {
                    GetExistedProblemAccounts(parameters.CustIds)
                   .GetProbAccountLosingClassification(parameters.FunctionId)
                   .InsertProblemAccountLogics(parameters.ServiceName, parameters.IsFromQueue)
                   .GetTWRobotUsers(parameters.CustIds).LogRescanPAData(robotUsers, parameters.FuncId_CheckRobotUser, parameters.ServiceName)
                   .InsertRobot(parameters)
                   .PushExternalData(parameters)
                   .RemoveOldProblemAccount();
                }

                GetAgentCusts(parameters.CustIds)
               .RescanNormalAccount(parameters);
            }

            return isSuccess;
        }

        public IRescanCustClassificationService GetExistedProblemAccounts(IEnumerable<long> custIds)
        {
            if (custIds != null && custIds.Any())
            {
                var result = problemAccountServices.GetExistedProblemAccounts(custIds);

                if (result.Any())
                {
                    problemAccounts = result.Where(c => !c.IsRobot).Select(c => new InsertProblemAccountEntity
                    {
                        CustID = c.CustID,
                        CTSCustID = c.CTSCustID,
                        RoleID = c.RoleID,
                        SubscriberID = c.SubscriberID,
                        CategoryID = c.CategoryID,
                        CategoryGroup = c.CategoryGroup,
                        IsLicensee = c.IsLicensee,
                        Remark = c.Remark,
                        IsDirectPA = c.IsDirect,
                        IsFromCTS = c.IsFromCTS,
                        IsFromTVS = c.IsFromTVS,
                        IsFromTW = c.IsFromTW,
                        IsDataChanged = false,
                        CreatedBy = StarixITId
                    });

                    robotClassificationPerformance = result.Where(c => c.IsRobot).Select(c => new InsertRobotEntity
                    {
                        CustID = c.CustID,
                        CategoryID = c.CategoryID,
                        IsFromTW = false,
                        ActiveDays = c.ActiveDays,
                        TurnoverRM = c.Turnover,
                        BetCount = c.BetCount,
                        WinlossRM = c.Winloss,
                        CreatedBy = StarixITId
                    });
                }
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

        public IRescanCustClassificationService InsertProblemAccountLogics(string serviceName, bool isRescanPAFromQueue)
        {
            if (problemAccounts.Any())
            {
                var result = problemAccountServices.InsertProblemAccountLogics(problemAccounts, problemAccountLosing, InsertPA_ByType.Category, isRescanPAFromQueue, serviceName);

                isSuccess = result.ErrorCode == 0;

                if (isSuccess && result.CustomerInfo != null && result.CustomerInfo.Any())
                {
                    var custIds = result.CustomerInfo.Select(c => c.CustID).Distinct();

                    custIdsToRemoveOldPA = custIds;

                    custIdsToRescanPA.AddRange(custIds);
                }
            }

            return this;
        }

        public IRescanCustClassificationService RemoveOldProblemAccount()
        {
            if (isSuccess && custIdsToRemoveOldPA != null && custIdsToRemoveOldPA.Any())
            {
                problemAccountServices.RemoveOldProblemAccount(custIdsToRemoveOldPA);
            }

            return this;
        }

        public IRescanCustClassificationService GetTWRobotUsers(IEnumerable<long> custIds)
        {
            if (isSuccess && custIds != null && custIds.Any())
            {
                var robots = rescanCustClassificationDataAccess.GetExistedTWRobotUser(string.Join(",", custIds));

                if (robots != null && robots.Any())
                {
                    robots.ToList().ForEach(c =>
                    {
                        robotUsers.Add(new InsertRobotEntity
                        {
                            CustID = c.CustId,
                            RobotCounter = c.Counter,
                            CategoryID = (int)Category.Robot,
                            IsFromTW = true,
                            CreatedBy = StarixITId
                        });
                    });
                }
            }

            return this;
        }

        public IRescanCustClassificationService InsertRobot(RescanParametersEntity paras)
        {
            var robotToInsert = new List<InsertRobotEntity>();

            if (robotUsers.Any())
            {
                robotToInsert.AddRange(robotUsers);
            }

            if (robotClassificationPerformance.Any())
            {
                robotToInsert.AddRange(robotClassificationPerformance);
            }

            var customerInfo = problemAccountServices.InsertRobotAccount(robotToInsert);

            if (isSuccess && customerInfo.Any())
            {
                var custIds = customerInfo.Select(c => c.CustID).Distinct();

                custIdsToRescanPA.AddRange(custIds);
            }

            return this;
        }

        public IRescanCustClassificationService PushExternalData(RescanParametersEntity parameters)
        {
            if (isSuccess && custIdsToRescanPA.Any())
            {
                Parallel.Invoke(() =>
                {
                    isSuccess = custClassToMainDBService.PushDataToExternal(custIdsToRescanPA.Distinct(), new PushDataToExternalEntity
                    {
                        ExternalBatchSize = parameters.ExternalBatchSize,
                        FeedSoon88API = parameters.FeedSoon88API,
                        IsPushCCToSoon88 = parameters.IsPushCCToSoon88,
                        IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                        IsWriteLog = parameters.IsWriteLog,
                        ServiceName = parameters.ServiceName
                    });
                },
                () =>
                {
                    antiFraudCenterServices.InsertAFCAccounts(custIdsToRescanPA.Distinct(), FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCInsertAccountURL);
                });
            }

            return this;
        }

        public bool PushExternalData(RescanParametersEntity parameters, IEnumerable<long> custIds)
        {
            var isSuccess = true;

            if (custIds != null && custIds.Any())
            {
                Parallel.Invoke(() =>
                {
                    isSuccess = custClassToMainDBService.PushDataToExternal(custIds.Distinct(), new PushDataToExternalEntity
                    {
                        ExternalBatchSize = parameters.ExternalBatchSize,
                        FeedSoon88API = parameters.FeedSoon88API,
                        IsPushCCToSoon88 = parameters.IsPushCCToSoon88,
                        IsUpdateDangerToMainDB = parameters.IsUpdateDangerToMainDB,
                        IsWriteLog = parameters.IsWriteLog,
                        ServiceName = parameters.ServiceName
                    });
                },
                () =>
                {
                    antiFraudCenterServices.InsertAFCAccounts(custIds.Distinct(), FromFlow.CTSServices, parameters.IsSyncAFCData, parameters.AFCInsertAccountURL);
                });
            }

            return isSuccess;
        }

        public IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds)
        {
            if (isSuccess && custIds != null && custIds.Any())
            {
                var data = rescanCustClassificationDataAccess.GetRoleCustomer(string.Join(",", custIds));

                if (data != null && data.Any())
                {
                    smaCustIds = data.Where(c => c.RoleID == (int)CustomerRole.Agent
                                              || c.RoleID == (int)CustomerRole.Master
                                              || c.RoleID == (int)CustomerRole.Super).Select(c => c.CustID)?.ToList();
                }
            }

            return this;
        }

        public IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras)
        {
            if (isSuccess && paras.CustIds != null && paras.CustIds.Any())
            {
                var custToRescanNormal = paras.CustIds.ToList();
                custToRescanNormal.RemoveAll(x => custIdsToRescanPA.Distinct().Contains(x));
                custToRescanNormal.RemoveAll(x => smaCustIds.Distinct().Contains(x));

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

        public IRescanCustClassificationService LogRescanPAData(IEnumerable<InsertRobotEntity> robotUsers, FunctionId funcId, string stageName)
        {
            custClassToMainDBService.LogInputData(new LogInputViewEntity { RobotUser = robotUsers }, funcId, stageName);

            return this;
        }

        public IEnumerable<long> CustIdsToRescanPA => custIdsToRescanPA;

        public IEnumerable<InsertRobotEntity> RobotUsers => robotUsers;
    }
}