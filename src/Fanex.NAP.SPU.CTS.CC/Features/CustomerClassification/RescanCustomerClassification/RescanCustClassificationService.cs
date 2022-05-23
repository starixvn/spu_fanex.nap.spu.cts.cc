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

        IRescanCustClassificationService InsertProblemAccountLogics(string serviceName);

        IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds);

        IRescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertRobot(RescanParametersEntity paras);

        IRescanCustClassificationService PushExternalData(RescanParametersEntity paras);

        IRescanCustClassificationService RescanNormalAccount(RescanParametersEntity paras);

        IRescanCustClassificationService LogRescanPAData(IEnumerable<InsertRobotEntity> RobotUser, FunctionId funcId, string stageName);

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
                   .InsertProblemAccountLogics(parameters.ServiceName)
                   .GetRobotUsers(parameters.CustIds).LogRescanPAData(robotUsers, parameters.FuncId_CheckRobotUser, parameters.ServiceName)
                   .InsertRobot(parameters)
                   .PushExternalData(parameters);
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
                        CategoryID = c.CategoryID,
                        CategoryGroup = c.CategoryGroup,
                        IsDirectPA = c.IsDirectPA,
                        Remark = c.Remark,
                        IsLicensee = c.IsLicensee,
                        RoleID = c.RoleID,
                        SubscriberID = c.SubscriberID,
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

        public IRescanCustClassificationService InsertProblemAccountLogics(string serviceName)
        {
            if (problemAccounts.Any())
            {
                var result = problemAccountServices.InsertProblemAccountLogics(problemAccounts, problemAccountLosing, InsertPA_ByType.Category ,serviceName);

                isSuccess = result.ErrorCode == 0;

                if (isSuccess && result.CustomerInfo != null && result.CustomerInfo.Any())
                {
                    var custIds = result.CustomerInfo.Select(c => c.CustID).Distinct();

                    custIdsToRescanPA.AddRange(custIds);
                }
            }

            return this;
        }

        public IRescanCustClassificationService GetRobotUsers(IEnumerable<long> custIds)
        {
            if (isSuccess && custIds != null && custIds.Any())
            {
                var robots = rescanCustClassificationDataAccess.GetExistedRobotUser(string.Join(",", custIds));

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

        public IRescanCustClassificationService PushExternalData(RescanParametersEntity paras)
        {
            if (isSuccess && custIdsToRescanPA.Any())
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

        public IRescanCustClassificationService LogRescanPAData(IEnumerable<InsertRobotEntity> robotUsers, FunctionId funcId, string stageName)
        {
            custClassToMainDBService.LogInputData(new LogInputViewEntity { RobotUser = robotUsers }, funcId, stageName);

            return this;
        }

        public IEnumerable<long> CustIdsToRescanPA => custIdsToRescanPA;

        public IEnumerable<InsertRobotEntity> RobotUsers => robotUsers;
    }
}