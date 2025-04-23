namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Sentry;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAssociatedGroupMonitorJobService
    {
        Task<bool> ScanAssociatedAccount();
    }

    public class AssociatedGroupMonitorJobService : IAssociatedGroupMonitorJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private const string serviceName = "Associated Group Monitor";
        private const string MarkPA_AssociatedGroup = "Associated Group";
        private const int DangerLevelABIOri = 2;
        private const int batchSize = 500;
        private readonly IAssociatedGroupMonitorService associatedGroupService;       
        private readonly ISystemParameterServices systemParameterServices;

        public AssociatedGroupMonitorJobService(ISentryClient sentryClient
            , IAppSettings appSettings
            , IAssociatedGroupMonitorService associatedGroupService        
            , ISystemParameterServices systemParameterServices)
        {
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.associatedGroupService = associatedGroupService;      
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
                        CategoryGroupID = custInfo.PACategoryID,
                        IsLicensee = x.IsLicensee,
                        IsMarkedDirectly = true,                       
                        IsFromCTS = true,
                        IsFromTVS = false,
                        IsFromTW = false,
                        CreatedBy = Constants.StarixITId,
                        GroupName = custInfo.GroupName,
                        Remark = MarkPA_AssociatedGroup + " - " + custInfo.GroupName                       
                    });

                    if (custSyncPAs.Any())
                    {
                        var custIdsResult = this.associatedGroupService.InsertProblemAccount(custSyncPAs, new PushDataToExternalEntity
                        {
                            RoleGroup = RoleGroupType.Member,
                            ExternalBatchSize = batchSize,
                            IsUpdateDangerToMainDB = true,
                            IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                            ServiceName = serviceName
                        });

                        var custGroupABIPAs = custSyncPAs.Select(x => x.CustID).Distinct().ToList();

                        if (custIdsResult != null && custIdsResult.Any())
                            custGroupABIPAs.RemoveAll(x => custIdsResult.Contains(x));

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
                            GroupID = custInfo.GroupID,
                            CreatedBy = Constants.StarixITId
                        });
                    }
                }

                if (dbResult.ErrorCode == 0)
                {
                    this.systemParameterServices.UpdateTimingExecution(custInfo.GroupID.ToString(), Constants.CTS_SystemParameters.AssociatedGroupLastGroupID);
                    this.systemParameterServices.UpdateTimingExecution(custInfo.LastCTSCustID.ToString(), Constants.CTS_SystemParameters.AssociatedGroupLastCTSCustID);
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
