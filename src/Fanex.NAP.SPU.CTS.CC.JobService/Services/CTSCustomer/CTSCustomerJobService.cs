namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
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

        Task<bool> UpdateUsername2ForCustMain(int batchSize);

        Task<bool> UpdateUsername2ForCustSub(int batchSize);

        Task<bool> InsertReactivatedCustomers();

        Task<bool> InactivateNewCustomers(int batchSize);
    }

    public class CTSCustomerJobService : ICTSCustomerJobService
    {
        private readonly ICTSCustomerService ctscustomerService;
        private readonly ISystemParameterServices systemParameterServices;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IRescanCustClassificationService rescanCustClassificationService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public CTSCustomerJobService(ISentryClient sentryClient,
            ICTSCustomerService ctscustomerService,
            ISystemParameterServices systemParameterServices,
            ICustClassToMainDBService custClassToMainDBService,
            IRescanCustClassificationService rescanCustClassificationService,
            IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.ctscustomerService = ctscustomerService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.rescanCustClassificationService = rescanCustClassificationService;
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

                var result = ctscustomerService.InsertNewCustomer(true, customers, new InsertNewCustRescanEntity
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


        #region Update UserName2

        public Task<bool> UpdateUsername2ForCustMain(int batchSize)
        {
            try
            {
                UInt64 lastCTSCustId = systemParameterServices.GetLastScannedId2(SystemParameters.CustomerNullUsername2_LastCTSCustID);

                ctscustomerService.UpdateUserName2Package(ref lastCTSCustId, false, batchSize);

                systemParameterServices.UpdateTimingExecution(lastCTSCustId.ToString(), SystemParameters.CustomerNullUsername2_LastCTSCustID);

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


        public Task<bool> UpdateUsername2ForCustSub(int batchSize)
        {
            try
            {
                UInt64 lastCTSCustId = 0;
                bool isContinue = false;

                do
                {
                    isContinue = ctscustomerService.UpdateUserName2Package(ref lastCTSCustId, true, batchSize);

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

        #endregion Update UserName2

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

                    var exitedCTSArchiveCustIDs = ctscustomerService.CheckExistCTSArchiveCustomer(custToReactive);

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
                        isSuccess = ctscustomerService.InsertNewCustomer(false, custInfo, new InsertNewCustRescanEntity
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

        public Task<bool> InactivateNewCustomers(int batchSize)
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

                        var rescanConfig = new RescanParametersEntity()
                        {
                            IsRescanPA = false,
                            IsToQueue = false,
                            FunctionId = FunctionId.InactivateNewCustomer,
                            RescanNormalType = RescanNormalType.Auto_Customer,
                            InternalBatchSize = batchSize,
                            ServiceName = "InactiveNewCustomer",
                            PushDataExternal = new PushDataToExternalEntity
                            {
                                IsPushCCToSoon88 = true,
                                FeedSoon88API = appSettings.Soon88FeedDataAPI,
                                IsUpdateDangerToMainDB = true,
                                ExternalBatchSize = batchSize,
                                ServiceName = "InactiveNewCustomer",
                                IsSyncAFCData = appSettings.IsSyncAFCData,
                                AFCInsertAccountURL = appSettings.AFCInsertAccountURL,
                                AFCDeleteAccountURL = appSettings.AFCDeleteAccountURL
                            }
                        };

                        rescanCustClassificationService.RescanCustomerClassification(custIDs, rescanConfig);

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

        #endregion Private Methods
    }
}