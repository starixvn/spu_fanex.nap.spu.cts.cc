namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.Notification.Services;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerJobService
    {
        Task<bool> InsertNewCustomer(int batchSize);

        Task<bool> UpdateCTSCustomerInfo(int packageSize);

        Task<bool> UpdateUsername2(int batchSize);

        Task<bool> MonitorInsertNewCustomer();

        Task<bool> InsertReactivatedCustomers();

        Task<bool> InactivateNewCustomers();
    }

    public class CTSCustomerJobService : ICTSCustomerJobService
    {
        private readonly ICTSCustomerService ctscustomerService;
        private readonly IDailyNormalClassificationServices dailyClassificationService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public CTSCustomerJobService(ISentryClient sentryClient,
            ICTSCustomerService ctscustomerService,
            IDailyNormalClassificationServices dailyClassificationService,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.ctscustomerService = ctscustomerService;
            this.dailyClassificationService = dailyClassificationService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.systemParameterServices = systemParameterServices;
            this.appSettings = appSettings;
        }

        public Task<bool> InsertNewCustomer(int batchSize)
        {
            try
            {
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
                    InsertVIPURL = appSettings.InsertVIPURL,
                    InsertBAURL = appSettings.InsertBAURL,
                    serviceName = "InsertNewCustomer"
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
                var isSuccess = true;
                var fromDate = ctscustomerService.GetLastScanTimeForCustomerMonitor(SystemParameters.InsertNewCustomerMonitoring);
                var toDate = fromDate.AddHours(2);
                if (toDate <= DateTime.Now.AddHours(-0.5))
                {
                    var custList = ctscustomerService.GetCustInfoFromMainDBForMonitor(fromDate, toDate).ToList();
                    custList.AddRange(ctscustomerService.GetCustSubInfoFromMainDBForMonitor(fromDate, toDate));
                    var missingCusts = ctscustomerService.CompareCustomerDataForMonitor(custList);
                    if (missingCusts.Any())
                    {
                        var mess = $"Scan from {fromDate:dd/MM/yyyy HH:mm:ss} to {toDate:dd/MM/yyyy HH:mm:ss} \n" +
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

                            isSuccess = ctscustomerService.InsertNewCustomer(data, new InsertNewCustRescanEntity
                            {
                                InternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                                ExternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                                IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                IsInsertMissingCust = true,
                                InsertVIPURL = appSettings.InsertVIPURL,
                                InsertBAURL = appSettings.InsertBAURL,
                                serviceName = "MonitorInsertNewCustomer"
                            });
                        }
                        else
                        {
                            ctscustomerService.InsertMissingCust(missingCusts, toDate);
                        }
                    }

                    if (isSuccess)
                    {
                        ctscustomerService.UpdateLastScanTimeForCustomerMonitor(toDate, SystemParameters.InsertNewCustomerMonitoring);
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
                    var custIDs = string.Join(",", custToReactive.Select(c => c.CustID).Distinct());

                    var exitedCTSArchiveCustIDs = ctscustomerService.CheckExistCTSArchiveCustomer(custIDs);

                    /* 1. Push CC to MainDB */
                    var isPushCCSuccess = this.PushDataToExternal(exitedCTSArchiveCustIDs, "InsertReactivatedCustomers");

                    /* 2. Insert New Customer */
                    var insertNewCusts = custToReactive.Select(x => x.CustID).Except(exitedCTSArchiveCustIDs);
                    var insertNewCustIDs = string.Join(",", insertNewCusts.Distinct());

                    var mainCusts = ctscustomerService.GetCustomerFromMainDBForUpdate(insertNewCustIDs, UpdateCustomer.ForNewCustomer);
                    var subCusts = ctscustomerService.GetCustSubFromMainDB(insertNewCustIDs);

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
                            InsertVIPURL = appSettings.InsertVIPURL,
                            InsertBAURL = appSettings.InsertBAURL,
                            serviceName = "ReactivatedCustomer"
                        });
                    }

                    if (isPushCCSuccess && isSuccess)
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
                var isContinue = true;

                do
                {
                    var inactivateCusts = ctscustomerService.GetInactivateNewCustomers();

                    if (inactivateCusts.CustInfos != null && inactivateCusts.CustInfos.Any())
                    {
                        var custIDs = inactivateCusts.CustInfos.Select(x => x.CustID).Distinct();

                        dailyClassificationService.ScanningDailyNormalClassification(custIDs, FunctionId.InactivateNewCustomer, false);

                        isContinue = true;
                    }
                    else
                    {
                        isContinue = false;
                    }

                    systemParameterServices.UpdateTimingExecution(inactivateCusts.LastCategoryID.ToString(), SystemParameters.InactiveNewMember_LastCategoryID);
                    systemParameterServices.UpdateTimingExecution(inactivateCusts.LastCustID.ToString(), SystemParameters.InactiveNewMember_LastCustID);

                    Thread.Sleep(10000);

                } while (isContinue);

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

        private bool PushDataToExternal(IEnumerable<long> custIDs, string serviceName)
        {           
            return custClassToMainDBService.PushDataToExternal(custIDs.Distinct(), new PushDataToExternalEntity
            {
                ExternalBatchSize = appSettings.InsertReactivatedCustomersBatchSize,
                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                IsPushCCToSoon88 = true,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = appSettings.InsertReactivatedCustomersIsWriteLog,
                ServiceName = serviceName
            });
        }

        private void NotifyTelegramMessage(string message)
        {
            var telegramService = new NotificationService();
            string proxy = appSettings.EvironmentName == "PROD" ? "10.2.252.100:8801" : null;
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", "-636418000", "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
               .Message(new MessageModel("Monitoring - Insert New Customer", DateTime.Now, appSettings.EvironmentName, message))
               .SendWithoutDelay();
        }

        #endregion Private Methods
    }
}