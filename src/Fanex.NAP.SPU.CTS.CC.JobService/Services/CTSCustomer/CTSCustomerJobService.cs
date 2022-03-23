namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.Notification.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sentry;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerJobService
    {
        Task<bool> InsertNewCustomer();

        Task<bool> UpdateCTSCustomerInfo(int packageSize);

        Task<bool> UpdateUsername2(int batchSize);

        Task<bool> MonitorInsertNewCustomer();

        Task<bool> InsertReactivatedCustomers();

        Task<bool> InactivateNewCustomers();

        Task<bool> InitInactivateNewCustomers();
    }

    public class CTSCustomerJobService : ICTSCustomerJobService
    {
        private readonly ICTSCustomerService ctscustomerService;
        private readonly IDailyClassificationServices dailyClassificationService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public CTSCustomerJobService(ISentryClient sentryClient, 
            ICTSCustomerService ctscustomerService,
            IDailyClassificationServices dailyClassificationService,
            ISystemParameterServices systemParameterServices,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.ctscustomerService = ctscustomerService;
            this.dailyClassificationService = dailyClassificationService;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
        }

        public Task<bool> InsertNewCustomer()
        {
            try
            {
                var batchSize = appSettings.TransformCTSCustomerBatchSize;
                var lastScanned = ctscustomerService.GetLastInsertCust();
                var customers = ctscustomerService.GetCustomerFromMainDBForInsertNew(batchSize, lastScanned.LastCustID);
                var custSubs = ctscustomerService.GetCustSubFromMainDB(batchSize, lastScanned.LastCustSubID).ToList();

                customers.CustomerInfos.AddRange(custSubs);

                var result = ctscustomerService.InsertNewCustomer(customers, new InsertNewCustRescanEntity
                {
                    InternalBatchSize = batchSize,
                    ExternalBatchSize = batchSize,
                    IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                    FeedSoon88API = appSettings.Soon88FeedDataAPI,
                    IsInsertMissingCust = false,
                    IsSyncAFCData = appSettings.IsSyncAFCData,
                    AFCInsertAccountURL = appSettings.AFCInsertAccountURL
                });

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
                var isAutoInsertMissing = appSettings.MonitorInsertNewCustIsAutoInsertMissingCust;

                var fromDate = ctscustomerService.GetLastScanTimeForCustomerMonitor(SystemParameters.InsertNewCustomerMonitoring);
                var toDate = fromDate.AddHours(2);
                if (toDate <= DateTime.Now.AddHours(-0.5))
                {
                    var custList = ctscustomerService.GetCustInfoFromMainDBForMonitor(fromDate, toDate).ToList();
                    custList.AddRange(ctscustomerService.GetCustSubInfoFromMainDBForMonitor(fromDate, toDate));
                    var missingCusts = ctscustomerService.CompareCustomerDataForMonitor(custList);
                    if (missingCusts.Any())
                    {
                        var mess = $"Scan from {fromDate.ToString("dd/MM/yyyy HH:mm:ss")} to {toDate.ToString("dd/MM/yyyy HH:mm:ss")} \n" +
                        $"{missingCusts.Count()} missing customers";

                        NotifyTelegramMessage(mess);
                        if (isAutoInsertMissing)
                        {
                            var missingCustData = custList.Where(c => missingCusts.Select(a => a.CustID).Contains(c.CustID)
                                                                    && missingCusts.Select(a => a.CustSubID).Contains(c.CustSubID));
                            var data = new InsertCustomerViewModel
                            {
                                CustomerInfos = missingCustData.ToList(),
                                LastInsertCustID = 0
                            };

                            ctscustomerService.InsertNewCustomer(data, new InsertNewCustRescanEntity
                            {
                                InternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                                ExternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                                IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                IsInsertMissingCust = true,
                                IsSyncAFCData = appSettings.IsSyncAFCData,
                                AFCInsertAccountURL = appSettings.AFCInsertAccountURL
                            });
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

        public Task<bool> InsertReactivatedCustomers()
        {
            try
            {
                var isSuccess = true;

                var lastScannedTime = systemParameterServices.GetLastScannedDateTime(SystemParameters.ReactivatedCustomerLastScannedDate);

                var custToReactive = ctscustomerService.GetReactivatedCustomers(lastScannedTime);

                if (custToReactive != null && custToReactive.Any())
                {
                    var custInfo = new InsertCustomerViewModel { CustomerInfos = new List<CustomerMainDBModel>(), LastInsertCustID = null };
                    string custIds = string.Join(",", custToReactive.Select(c=>c.CustID).Distinct());

                    var mainCusts = ctscustomerService.GetCustomerFromMainDBForUpdate(custIds, UpdateCustomer.ForNewCustomer);
                    var subCusts = ctscustomerService.GetCustSubFromMainDB(custIds);

                    custInfo.CustomerInfos.AddRange(mainCusts);
                    custInfo.CustomerInfos.AddRange(subCusts);

                    if (custInfo != null && custInfo.CustomerInfos.Any())
                    {
                        isSuccess = ctscustomerService.InsertNewCustomer(custInfo, new InsertNewCustRescanEntity
                        {
                            InternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                            ExternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                            IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                            FeedSoon88API = appSettings.Soon88FeedDataAPI,
                            IsInsertMissingCust = false,
                            IsSyncAFCData = appSettings.IsSyncAFCData,
                            AFCInsertAccountURL = appSettings.AFCInsertAccountURL
                        });
                    }

                    if (isSuccess)
                    {
                        isSuccess = systemParameterServices.UpdateTimingExecution(custToReactive.Select(c => c.LogTime).Max().ToString(CONVERT_TO_SQL_DATETIME), SystemParameters.ReactivatedCustomerLastScannedDate);
                    }
                }

                return Task.FromResult(isSuccess);
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

        public Task<bool> InactivateNewCustomers()
        {
            try
            {
                var isRunInactivate = true;

                do
                {
                    var inactivateCusts = ctscustomerService.GetInactivateNewCustomers();

                    if (inactivateCusts.CustIDs != null && inactivateCusts.CustIDs.Any())
                    {
                        dailyClassificationService.ClassifyCustomerGeneralCategoryDaily(inactivateCusts.CustIDs, FunctionId.InactivateNewCustomer, false);

                        ctscustomerService.UpdateStatusInactivateNewCustomers(inactivateCusts.LastCTSCustID, inactivateCusts.LastCreatedDate);

                        isRunInactivate = true;

                        Thread.Sleep(10000);
                    }
                    else
                    {
                        isRunInactivate = false;
                    }
                 
                } while (isRunInactivate);
                    
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

        public Task<bool> InitInactivateNewCustomers()
        {
            try
            {
                var inactivateCusts = ctscustomerService.GetInactivateNewCustomers();

                if (inactivateCusts.CustIDs != null && inactivateCusts.CustIDs.Any())
                {
                    dailyClassificationService.ClassifyCustomerGeneralCategoryDaily(inactivateCusts.CustIDs, FunctionId.InactivateNewCustomer, false);

                    ctscustomerService.UpdateStatusInactivateNewCustomers(inactivateCusts.LastCTSCustID, inactivateCusts.LastCreatedDate);
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

        #region Private Methods
        private void NotifyTelegramMessage(string message)
        {
            var telegramService = new NotificationService();
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-636418000", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken")
               .Message(new MessageModel("Monitoring - Insert New Customer", DateTime.Now, appSettings.EvironmentName, message))
               .SendWithoutDelay(isInfo: true);
        }            

        #endregion
    }
}