namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification
{
    using Fanex.Data;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.DataLogTracking;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanCustClassificationService
    {
        IRescanCustClassificationService Start();

        bool RescanCustomerClassification(IEnumerable<long> custIds, RescanParametersEntity paras);

        IRescanCustClassificationService GetExistedProblemAccounts(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertProblemAccountLogics(string serviceName, bool isRescanPAFromQueue, FunctionId functionId);

        IRescanCustClassificationService RemoveOldProblemAccount();

        IRescanCustClassificationService GetAgentCusts(IEnumerable<long> custIds);

        IRescanCustClassificationService GetTWRobotUsers(IEnumerable<long> custIds);

        IRescanCustClassificationService InsertRobot();

        IRescanCustClassificationService PushExternalProblemAccounts(PushDataToExternalEntity paras);

        IRescanCustClassificationService RescanNormalAccount(IEnumerable<long> custIds, RescanParametersEntity paras);

        IRescanCustClassificationService RemovePAForSMA(PushDataToExternalEntity parameters);

        IRescanCustClassificationService LogRescanPAData(IEnumerable<InsertRobotEntity> robotUsers, FunctionId funcId, string stageName);
       
        IEnumerable<long> CustIdsToRescanPA { get; }

        IEnumerable<InsertRobotEntity> RobotUsers { get; }

        IEnumerable<InsertProblemAccountEntity> ExistedPA { get; }

        IEnumerable<InsertRobotEntity> ExistedRobot { get; }

        IEnumerable<InsertRobotEntity> TWRobots { get; }
    }

    public class RescanCustClassificationService : IRescanCustClassificationService
    {
        private readonly IRescanCustClassificationDataAccess rescanCustClassificationDataAccess;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;
        private readonly IDailyNormalClassificationServices dailyClassificationServices;
        private readonly IDataLogTrackingServices dataLogTrackingServices;
        private IEnumerable<InsertProblemAccountEntity> problemAccounts;
        private IEnumerable<InsertRobotEntity> robotClassification;
        private List<InsertRobotEntity> robotUsers;
        private List<long> custIdsToRescanPA;
        private List<long> custIdsToRemoveOldPA;
        private List<long> smaCustIds;
        private bool isSuccess;

        public RescanCustClassificationService(
            IRescanCustClassificationDataAccess rescanCustClassificationDataAccess,
            ISentryClient sentryClient,
            IProblemAccountServices problemAccountServices,
            ICustClassToMainDBService custClassToMainDBService,
            IDailyNormalClassificationServices dailyClassificationServices,
            IDataLogTrackingServices dataLogTrackingServices)
        {
            this.rescanCustClassificationDataAccess = rescanCustClassificationDataAccess;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.sentryClient = sentryClient;
            this.dailyClassificationServices = dailyClassificationServices;
            this.dataLogTrackingServices = dataLogTrackingServices;
        }

        public IRescanCustClassificationService Start()
        {
            this.problemAccounts = Enumerable.Empty<InsertProblemAccountEntity>();
            this.robotUsers = new List<InsertRobotEntity>();
            this.robotClassification = Enumerable.Empty<InsertRobotEntity>();
            this.custIdsToRescanPA = new List<long>();
            this.custIdsToRemoveOldPA = new List<long>();
            this.smaCustIds = new List<long>();
            this.isSuccess = true;

            return this;
        }

        public bool RescanCustomerClassification(IEnumerable<long> custIds, RescanParametersEntity parameters)
        {
            if (custIds != null && custIds.Any())
            {
                // Write log for test
                if (parameters.IsRescanPA)
                {
                    Utilities.LogSentryInfo("Affected Downline", "Is Rescan PA", custIds, true, sentryClient);
                }
                else
                {
                    Utilities.LogSentryInfo("Affected Downline", "Not Rescan PA", custIds, true, sentryClient);
                }
                // End write log for test

                Start();
              
                if (parameters.IsRescanPA)
                {
                    GetExistedProblemAccounts(custIds)
                   .InsertProblemAccountLogics(parameters.ServiceName, parameters.IsToQueue, parameters.FunctionId)
                   .GetTWRobotUsers(custIds)
                   .LogRescanPAData(robotUsers, parameters.FuncId_CheckRobotUser, parameters.ServiceName)
                   .InsertRobot()
                   .PushExternalProblemAccounts(parameters.PushDataExternal)
                   .RemoveOldProblemAccount();
                }

                GetAgentCusts(custIds)
               .RescanNormalAccount(custIds, parameters)
               .RemovePAForSMA(parameters.PushDataExternal);
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
                        TVSRequestID = c.TVSRequestID,
                        IsParlay = c.IsParlay,
                        SportType = c.SportType,
                        IssueTypeID = c.IssueTypeID,
                        IsFromTW = c.IsFromTW,
                        IsDataChanged = false,
                        LastModifiedDate = c.LastModifiedDate,
                        CreatedBy = StarixITId
                    });

                    robotClassification = result.Where(c => c.IsRobot).Select(c => new InsertRobotEntity
                    {
                        CustID = c.CustID,
                        CategoryID = c.CategoryID,
                        IsDirect = c.IsDirect,
                        IsFromCTS = c.IsFromCTS,
                        IsFromTW = c.IsFromTW,
                        IsFromAI = c.IsFromAI,
                        IsFromImperva = c.IsFromImperva,
                        IsFromTVS = c.IsFromTVS,
                        TVSRequestID = c.TVSRequestID,
                        IsParlay = c.IsParlay,
                        SportType = c.SportType,
                        IssueTypeID = c.IssueTypeID,
                        CreatedBy = StarixITId,
                        Remark = c.Remark
                    });
                }
            }

            return this;
        }

        public IRescanCustClassificationService InsertProblemAccountLogics(string serviceName, bool isToQueue, FunctionId functionId)
        {
            if (problemAccounts.Any())
            {
                var custIds = problemAccountServices.InsertProblemAccountPackage(problemAccounts, functionId, InsertPA_ByType.Category, isToQueue, serviceName);

                if (custIds != null && custIds.Any())
                {
                    custIdsToRemoveOldPA.AddRange(custIds.Distinct());

                    custIdsToRescanPA.AddRange(custIds.Distinct());
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
                            IsFromTW = true,
                            IsDirect = true,
                            CreatedBy = StarixITId
                        });
                    });
                }
            }

            return this;
        }

        public IRescanCustClassificationService InsertRobot()
        {
            var robotToInsert = new List<InsertRobotEntity>();

            if (robotUsers.Any())
            {
                robotToInsert.AddRange(robotUsers);
            }

            if (robotClassification.Any())
            {
                robotToInsert.AddRange(robotClassification);
            }

            var customerInfo = problemAccountServices.InsertRobotAccountPackage(robotToInsert, true);

            if (isSuccess && customerInfo.Any())
            {
                var custIds = customerInfo.Select(c => c.CustID).Distinct();

                custIdsToRemoveOldPA.AddRange(custIds);

                custIdsToRescanPA.AddRange(custIds);
            }

            return this;
        }

        public IRescanCustClassificationService PushExternalProblemAccounts(PushDataToExternalEntity parameters)
        {
            if (isSuccess && custIdsToRescanPA.Any())
            {
                isSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(custIdsToRescanPA.Distinct(), parameters);
            }

            return this;
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

        public IRescanCustClassificationService RescanNormalAccount(IEnumerable<long> custIds, RescanParametersEntity paras)
        {
            if (isSuccess && custIds != null && custIds.Any())
            {
                var custToRescanNormal = custIds.ToList();
                custToRescanNormal.RemoveAll(x => custIdsToRescanPA.Distinct().Contains(x));
                custToRescanNormal.RemoveAll(x => smaCustIds.Distinct().Contains(x));
              
                if (custToRescanNormal.Any())
                {
                    var custIdsChunk = custToRescanNormal.Chunk(paras.InternalBatchSize);

                    foreach (var batchCustIds in custIdsChunk)
                    {
                        dailyClassificationServices.ScanningDailyNormalClassification(batchCustIds, paras.FunctionId);
                    }
                }
            }

            return this;
        }

        public IRescanCustClassificationService RemovePAForSMA(PushDataToExternalEntity parameters)
        {
            if (isSuccess && smaCustIds != null && smaCustIds.Any())
            {
                var custToRemovePA = smaCustIds.Distinct().Except(custIdsToRescanPA);

                if (custToRemovePA.Any())
                {
                    isSuccess = custClassToMainDBService.PushExternalProblemAcountPackage(custToRemovePA?.Distinct(), parameters);
                }
            }

            return this;
        }

        public IRescanCustClassificationService LogRescanPAData(IEnumerable<InsertRobotEntity> robotUsers, FunctionId funcId, string stageName)
        {
            dataLogTrackingServices.LogInputData(new LogInputViewEntity { RobotUser = robotUsers }, funcId, stageName);

            return this;
        }        

        public IEnumerable<long> CustIdsToRescanPA => custIdsToRescanPA;

        public IEnumerable<InsertRobotEntity> RobotUsers => robotUsers;

        public IEnumerable<InsertProblemAccountEntity> ExistedPA => problemAccounts;

        public IEnumerable<InsertRobotEntity> ExistedRobot => robotClassification;

        public IEnumerable<InsertRobotEntity> TWRobots => robotUsers;
    }
}