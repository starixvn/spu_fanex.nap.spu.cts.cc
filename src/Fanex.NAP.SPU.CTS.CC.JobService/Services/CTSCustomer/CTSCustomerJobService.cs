namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.Notification.Services;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerJobService
    {
        Task<bool> InsertNewCustomer();

        Task<bool> UpdateCTSCustomerInfo(int packageSize);

        Task<bool> UpdateUsername2(int batchSize);

        Task<bool> MonitorInsertNewCustomer();
    }

    public class CTSCustomerJobService : ICTSCustomerJobService
    {
        private readonly ICTSCustomerService ctscustomerService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public CTSCustomerJobService(ISentryClient sentryClient, ICTSCustomerService ctscustomerService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.ctscustomerService = ctscustomerService;
            this.appSettings = appSettings;
        }

        public Task<bool> InsertNewCustomer()
        {
            try
            {
                var isWriteLog = appSettings.TransformCTSCustomerIsWriteLog;
                var soon88Url = appSettings.Soon88FeedDataAPI;
                var batchSize = appSettings.TransformCTSCustomerBatchSize;
                var logContent = $"Start InsertNewCustomer - Batch size: {batchSize} - Time: {DateTime.Now} \n";

                var lastScanned = ctscustomerService.GetLastInsertCust();
                var customers = ctscustomerService.GetCustomerFromMainDBForInsertNew(batchSize, lastScanned.LastCustID);
                var custSubs = ctscustomerService.GetCustSubFromMainDB(batchSize, lastScanned.LastCustSubID).ToList();
                logContent += $"LastCustID: {lastScanned.LastCustID} - Quantity of customer list: {customers.CustomerInfos.Count}\n";
                logContent += $"LastCustSubID: {lastScanned.LastCustSubID} - Quantity of custsub list: {custSubs.Count}\n";
                customers.CustomerInfos.AddRange(custSubs);
                logContent += $"Quantity of total list: {customers.CustomerInfos.Count}\n";

                var result = ctscustomerService.InsertNewCustomer(customers, isWriteLog, batchSize, soon88Url, false);
                sentryClient.CaptureMessage(logContent, Sentry.Protocol.SentryLevel.Info);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateCTSCustomerInfo(int packageSize)
        {
            TranformCustModel updatedCustFromCustInfo;
            TranformCustModel updatedCustFromCustomer;
            TranformCustModel updatedCustFromProductStatus;

            try
            {
                var isWriteLog = appSettings.TransformCTSCustomerIsWriteLog;
                var output = ctscustomerService.GetLastUpdatedTime();
                updatedCustFromCustInfo = ctscustomerService.GetUpdatedCustFromCustInfoHistory(output.LastUpdateTimeCustInfo, output.LastUpdateCustIDCustInfo, packageSize);
                updatedCustFromCustomer = ctscustomerService.GetUpdatedCustFromCustomerHistory(output.LastUpdateTimeCustomer, output.LastUpdateCustIDCustomer, packageSize);
                updatedCustFromProductStatus = ctscustomerService.GetUpdatedCustFromProductStatusHistory(output.LastUpdateTimeCustProductStatus, output.LastUpdateCustIDCustProductStatus, packageSize);

                if (ctscustomerService.IsValidData(updatedCustFromCustomer)
                    || ctscustomerService.IsValidData(updatedCustFromCustInfo)
                    || ctscustomerService.IsValidData(updatedCustFromProductStatus))
                {
                    ctscustomerService.UpdateCustomer(updatedCustFromCustInfo, UpdateCustomer.FromCustInfo, isWriteLog);
                    ctscustomerService.UpdateCustomer(updatedCustFromCustomer, UpdateCustomer.FromCustomer, isWriteLog);
                    ctscustomerService.UpdateCustomer(updatedCustFromProductStatus, UpdateCustomer.FromProductStatus, isWriteLog);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateUsername2(int batchSize)
        {
            try
            {
                long preCTSCustId = 0;
                List<GetCustIdModel> custsNullUsername2;

                do
                {
                    custsNullUsername2 = ctscustomerService.GetCustWithNullUserName2(batchSize, preCTSCustId);
                    if (custsNullUsername2 is not null && custsNullUsername2.Any())
                    {
                        var custFromMainDB = ctscustomerService.GetCustWithHavingUserName2(custsNullUsername2);
                        preCTSCustId = custsNullUsername2.Max(c => c.CTSCustID);
                        ctscustomerService.UpdateUsername2(custFromMainDB);
                    }
                } while (custsNullUsername2 is not null && custsNullUsername2.Any());

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        public Task<bool> MonitorInsertNewCustomer()
        {
            try
            {
                var isWriteLog = appSettings.TransformCTSCustomerIsWriteLog;
                var soon88Url = appSettings.Soon88FeedDataAPI;
                var batchSize = appSettings.TransformCTSCustomerBatchSize;
                var isAutoInsertMissing = appSettings.MonitorInsertNewCustIsAutoInsertMissingCust;

                var fromDate = ctscustomerService.GetLastScanTimeForCustomerMonitor(SystemParameters.InsertNewCustomerMonitoring);
                var toDate = fromDate.AddHours(2);
                toDate = toDate > DateTime.Now.AddHours(0.5) ? DateTime.Now.AddHours(-4) : toDate;
                if (toDate <= DateTime.Now.AddHours(0.5))
                {
                    var custList = ctscustomerService.GetCustInfoFromMainDBForMonitor(fromDate, toDate).ToList();
                    custList.AddRange(ctscustomerService.GetCustSubInfoFromMainDBForMonitor(fromDate, toDate));
                    var missingCusts = ctscustomerService.CompareCustomerDataForMonitor(custList);
                    if (missingCusts.Any())
                    {
                        var mess = $"Scan from {fromDate.ToString("dd/MM/yyyy HH:mm:ss")} to {toDate.ToString("dd/MM/yyyy HH:mm:ss")} \n" +
                        $"{missingCusts.Count()} missing customers";

                        //NotifyTelegramMessage(mess);
                        if (isAutoInsertMissing)
                        {
                            var missingCustData = custList.Where(c => missingCusts.Select(a => a.CustID).Contains(c.CustID)
                                                                    && missingCusts.Select(a => a.CustSubID).Contains(c.CustSubID));
                            var data = new UpdateCustomerModel
                            {
                                CustomerInfos = missingCustData.ToList(),
                                LastInsertCustID = 0
                            };
                            ctscustomerService.InsertNewCustomer(data, isWriteLog, batchSize, soon88Url, true);
                        }
                        else
                        {
                            ctscustomerService.InsertMissingCust(missingCusts, toDate);
                        }
                    }

                    ctscustomerService.UpdateLastScanTimeForCustomerMonitor(toDate, SystemParameters.InsertNewCustomerMonitoring);
                }
                

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();

                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        private void NotifyTelegramMessage(string message)
        {
            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-636418000", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken")
               .Message(new MessageModel("Monitoring - Insert New Customer", DateTime.Now, appSettings.EvironmentName, message))
               .SendWithoutDelay(isInfo: true);
        }
    }
}