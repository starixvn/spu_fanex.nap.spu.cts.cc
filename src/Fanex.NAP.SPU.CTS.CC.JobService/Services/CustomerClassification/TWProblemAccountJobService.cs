namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWProblemAccountJobService
    {
        Task<bool> Execute();
    }

    public class TWProblemAccountJobService : ITWProblemAccountJobService
    {
        private readonly ISentryClient sentryClient;
        private readonly ITWProblemAccountService twProblemAccService;
        private readonly IDailyProblemClassificationServices dailyPCServices;
        private readonly IProblemAccountServices paServices;
        private readonly IAppSettings appSettings;
        private const string ServiceName = "TWProblemAccountInsert";
        private const int batchSize = 500;

        public TWProblemAccountJobService(ITWProblemAccountService twProblemAccService
            , IProblemAccountServices paServices
            , IDailyProblemClassificationServices dailyPCServices
            , ISentryClient sentryClient
            , IAppSettings appSettings)
        {
            this.twProblemAccService = twProblemAccService;
            this.paServices = paServices;
            this.dailyPCServices = dailyPCServices;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public Task<bool> Execute()
        {
            try
            {
                uint lastCustID;
                var scanningCust = twProblemAccService.ScanSystemFormulaBetMonitor(out lastCustID);
                while (scanningCust != null && scanningCust.Any())
                {
                    var perfCust = twProblemAccService.GetCustomerPerformance(string.Join(",", scanningCust.Select(c => c.CustID)));
                    var fullData = (from sCust in scanningCust
                                    join pCust in perfCust on sCust.CustID equals pCust.CustID into temp
                                    from pCust in temp.DefaultIfEmpty()
                                    select new RescanTWProblemAccountModel
                                    {
                                        CustID = sCust.CustID,
                                        Username = sCust.Username,
                                        IsMonitorCategoryGroup = sCust.IsMonitorCategoryGroup,
                                        IsMonitorSite = sCust.IsMonitorSite,
                                        Winloss = pCust == null ? 0 : pCust.Winloss,
                                        RoleId = sCust.RoleId,
                                        ReasonId = sCust.ReasonId,
                                        PreventActionId = sCust.PreventActionId,
                                        Comment = sCust.Comment,
                                        VoidTime = sCust.VoidTime,
                                        LimitAmount = sCust.LimitAmount,
                                        UserId = sCust.UserId,
                                        Ip = sCust.Ip,
                                        InsertTime = sCust.InsertTime
                                    }).ToList();

                    var markPAList = fullData.Where(c => c.Winloss >= 0 && !c.IsMonitorCategoryGroup);
                    var markPAListGroupByInsertTime = markPAList.GroupBy(c => c.InsertTime);
                    foreach (var paList in markPAListGroupByInsertTime)
                    {
                        if (paList.Any())
                        {
                            var firtItem = paList.FirstOrDefault();
                            
                            var custIds = paList.Select(x => (long)x.CustID);

                            var paLosing = dailyPCServices.GetProblemAccountLosingClassification(custIds, FunctionId.TWProblemAccountInsert);

                            var pas = paList.Select(x => new InsertProblemAccountEntity
                            {
                                CustID = x.CustID,
                                ReasonID = x.ReasonId,
                                Remark = x.Comment,
                                IsDirectPA = true,
                                IsFromTW = true,
                                CreatedBy = x.UserId
                            });

                            paServices.InsertProblemAccountLogics(pas, paLosing, InsertPA_ByType.Reason, true, ServiceName);

                            dailyPCServices.PushCCToExternal(new PushDataToExternalEntity
                            {
                                ExternalBatchSize = batchSize,
                                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                IsPushCCToSoon88 = true,
                                IsUpdateDangerToMainDB = true,
                                IsWriteLog = appSettings.ProblemAccountIsWriteLog,
                                ServiceName = ServiceName
                            });

                            twProblemAccService.RemoveSystemFormulaBetCustomer(string.Join(",", paList.Select(c => c.CustID)));
                        }
                    }

                    twProblemAccService.ScanSystemFormulaBetMonitorComplete(lastCustID);

                    // prepare for new Batch
                    scanningCust = twProblemAccService.ScanSystemFormulaBetMonitor(out lastCustID);
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"CTS - TWProblemAccountService - DailyScan exeption message:  {logMessage}");

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

    }
}
