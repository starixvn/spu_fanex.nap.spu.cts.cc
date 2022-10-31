namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.AssociatedGroupMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
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
        private const string serviceName = "ArchiveAssociationCustomer";
        private const int DangerLevelABI = 2;
        private readonly IAssociatedGroupMonitorService associatedGroupService;
        private readonly IProblemAccountServices problemAccountServices;
        private readonly ISystemParameterServices systemParameterServices;

        public AssociatedGroupMonitorJobService(ISentryClient sentryClient
            , IAssociatedGroupMonitorService associatedGroupService
            , IProblemAccountServices problemAccountServices
            , ISystemParameterServices systemParameterServices)
        {
            this.sentryClient = sentryClient;
            this.associatedGroupService = associatedGroupService;
            this.problemAccountServices = problemAccountServices;
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
                        CreatedBy = Constants.StarixITId
                    });

                    var custIdsResult = problemAccountServices.InsertProblemAccountPackage(custSyncPAs
                        , Shares.Constants.FunctionId.ScanAssociatedAccount
                        , Shares.Constants.InsertPA_ByType.Category
                        , false
                        , serviceName);
                    
                    var ctsCustIDs = string.Join(",", custInfo.ScanAssociatedAccountList.Select(x => x.CTSCustID).Distinct());

                    var custSyncABIs = custInfo.ScanAssociatedAccountList.Where(x => !x.IsSyncPA).Select(x => x.CustID);

                    var custInfoMainDBs = this.associatedGroupService.GetCustomerInfoMainDB(string.Join(",", custSyncABIs), DangerLevelABI);

                    dbResult = this.associatedGroupService.UpdateDangerLevelToMainDB(custInfoMainDBs, custInfo.ABI, custInfo.Ori);

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
