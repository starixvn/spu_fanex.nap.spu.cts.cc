namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IAssociatedGroupMonitorJobService
    {
        Task<bool> ScanAssociatedAccount();
    }

    public class AssociatedGroupMonitorJobService : IAssociatedGroupMonitorJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private const string serviceName = "ArchiveAssociationCustomer";
        private const string MarkPA_AssociatedGroup = "Associated Group";
        private const int DangerLevelABIOri = 2;
        private const int batchSize = 500;
        private readonly IAssociatedGroupMonitorService associatedGroupService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISystemParameterServices systemParameterServices;

        public AssociatedGroupMonitorJobService(ISentryClient sentryClient
            , IAppSettings appSettings
            , IAssociatedGroupMonitorService associatedGroupService
            , IProblemAccountServices problemAccountServices
            , ICustClassToMainDBService custClassToMainDBService
            , ISystemParameterServices systemParameterServices)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.associatedGroupService = associatedGroupService;
            this.problemAccountServices = problemAccountServices;
            this.custClassToMainDBService = custClassToMainDBService;
            this.systemParameterServices = systemParameterServices;
        }

        public Task<bool> ScanAssociatedAccount()
        {
            try
            {
                var dbResult = new DBResult { ErrorCode = 0 };

                var custInfo = this.associatedGroupService.ScanAssociatedAccount();

                if (custInfo.ScanAssociatedAccountList != null && custInfo.ScanAssociatedAccountList.Any())
                {                    
                    var custSyncPAs = custInfo.ScanAssociatedAccountList.Where(x => x.IsSyncPA).Select(x => new InsertProblemAccountEntity
                    {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        RoleID = x.RoleID,
                        SubscriberID = x.SubscriberID,
                        CategoryID = custInfo.PACategoryID,
                        CategoryGroup = custInfo.PACategoryID,
                        IsLicensee = x.IsLicensee,
                        IsDirectPA = true,                       
                        IsFromCTS = true,
                        IsFromTVS = false,
                        IsFromTW = false,
                        CreatedBy = Constants.StarixITId,
                        Remark = MarkPA_AssociatedGroup + " - " + custInfo.GroupName                       
                    });

                    if (custSyncPAs.Any())
                    {
                        var custGroupABIPAs = custSyncPAs.Select(x => x.CustID).Distinct();

                        problemAccountServices.InsertProblemAccountPackage(custSyncPAs
                           , Shares.Constants.FunctionId.ScanAssociatedAccount
                           , Shares.Constants.InsertPA_ByType.Category
                           , false
                           , serviceName);

                        custClassToMainDBService.PushExternalProblemAcountPackage(custGroupABIPAs, new PushDataToExternalEntity
                        {
                            ExternalBatchSize = batchSize,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsPushCCToSoon88 = true,
                            IsUpdateDangerToMainDB = true,
                            IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                            ServiceName = serviceName
                        });                        

                        var custUpdateGroupABI = this.associatedGroupService.GetCustomerInfoMainDB(string.Join(",", custGroupABIPAs), DangerLevelABIOri);

                        dbResult = this.associatedGroupService.UpdateABIToMainDB(custUpdateGroupABI, custInfo.ABI);
                    }                   
                    
                    var ctsCustIDs = string.Join(",", custInfo.ScanAssociatedAccountList.Select(x => x.CTSCustID).Distinct());

                    var custSyncGroupABI = custInfo.ScanAssociatedAccountList.Where(x => !x.IsSyncPA).Select(x => x.CustID).Distinct();

                    if (custSyncGroupABI.Any())
                    {
                        var custUpdateABI = this.associatedGroupService.GetCustomerInfoMainDB(string.Join(",", custSyncGroupABI), DangerLevelABIOri);

                        dbResult = this.associatedGroupService.UpdateABIToMainDB(custUpdateABI, custInfo.ABI);
                    }                   

                    var custSyncOri = custInfo.ScanAssociatedAccountList.Select(x => x.CustID).Distinct();

                    var custUpdateOri = this.associatedGroupService.GetCustomerInfoMainDB(string.Join(",", custSyncOri), DangerLevelABIOri);

                    dbResult = this.associatedGroupService.UpdateOriToMainDB(custUpdateOri, custInfo.Ori);

                    if (dbResult.ErrorCode == 0)
                    {
                        dbResult = this.associatedGroupService.AddAssociatedAccount(new AddAssociatedAccountModel
                        {
                            CTSCustIDs = ctsCustIDs,
                            IsAuto = true,
                            GroupID = custInfo.GroupID
                        });
                    }
                }

                if (dbResult.ErrorCode == 0)
                {
                    this.systemParameterServices.UpdateTimingExecution(custInfo.GroupID.ToString(), Constants.SystemParameters.AssociatedGroupLastGroupID);
                    this.systemParameterServices.UpdateTimingExecution(custInfo.LastCTSCustID.ToString(), Constants.SystemParameters.AssociatedGroupLastCTSCustID);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
