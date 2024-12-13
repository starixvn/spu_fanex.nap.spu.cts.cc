namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CTSCustomer
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models.ArchiveCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
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

        Task<bool> InsertMissingCustomer(int batchSize);

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
                var isSuccess = true;

                var lastScanned = ctscustomerService.GetLastInsertCust();
                var customers = ctscustomerService.GetCustomerFromMainDBForInsertNew(batchSize, lastScanned.LastCustID);

                if (customers?.CustomerInfos != null && customers.CustomerInfos.Any())
                {
                    var custSubs = ctscustomerService.GetCustSubFromMainDBByLastCustSub(batchSize, lastScanned.LastCustSubID);

                    customers.CustomerInfos.AddRange(custSubs);

                    isSuccess = ctscustomerService.InsertNewCustomer(true, customers, new InsertNewCustRescanEntity
                    {
                        InternalBatchSize = batchSize,
                        ExternalBatchSize = batchSize,
                        IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                        IsInsertMissingCust = false,
                        InsertVIPURL = appSettings.InsertVIPURL,
                        InsertBAURL = appSettings.InsertBAURL,
                        serviceName = "InsertNewCustomer"
                    });
                }

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
                      .ToString();
                 
                sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                return Task.FromResult(false);
            }
        }

        public Task<bool> InsertMissingCustomer(int batchSize)
        {
            try
            {
                var isSuccess = true;
                var custInfo = new InsertCustomerViewModel();

                var data = ctscustomerService.GetMissingCustomers();

                custInfo = ctscustomerService.GetMissingCustInfo(data.CustIDs);

                if (custInfo.CustomerInfos.Any())
                {
                    isSuccess = ctscustomerService.InsertNewCustomer(true, custInfo, new InsertNewCustRescanEntity
                    {
                        InternalBatchSize = batchSize,
                        ExternalBatchSize = batchSize,
                        IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                        IsInsertMissingCust = true,
                        InsertVIPURL = appSettings.InsertVIPURL,
                        InsertBAURL = appSettings.InsertBAURL,
                        serviceName = "InsertMissingCustomer"
                    });
                }

                return Task.FromResult(isSuccess);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
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
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
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
                UInt64 lastCTSCustId = systemParameterServices.GetLastScannedId2(CTS_SystemParameters.CustomerNullUsername2_LastCTSCustID);

                ctscustomerService.UpdateUserName2Package(ref lastCTSCustId, false, batchSize);

                systemParameterServices.UpdateTimingExecution(lastCTSCustId.ToString(), CTS_SystemParameters.CustomerNullUsername2_LastCTSCustID);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection(Constants.DataInfo, DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection(Constants.ExceptionInfo, CommonInfoGetters.GetExceptionInfo(ex))
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public Task<bool> InsertReactivatedCustomers()
        {
            try
            {
                var isSuccess = true;

                var lastScannedTime = systemParameterServices.GetLastScannedDateTime(CTS_SystemParameters.ReactivatedCustomerLastScannedDate);

                var maxLogTime = DateTime.MaxValue;
                var custInfoTime = DateTime.MaxValue;
                var requestTime = DateTime.Now;

                // step 1 get
                var custToReactive = ctscustomerService.GetReactivatedCustomers(lastScannedTime);

                // step 2
                var ctsArchiveCustIDs = CheckExistCTSArchiveCustomerAndPushCC(custToReactive);

                custInfoTime = DateTime.Now;

                if (ctsArchiveCustIDs.IsPushCCSuccess)
                {
                    // step 3
                    var isInsertReactivedCust = InsertNewCustFromReactivedCust(ctsArchiveCustIDs.NotExistCusts);

                    // step 4 update scantime for next
                    if (isInsertReactivedCust)
                    {
                        maxLogTime = custToReactive.Select(c => c.LogTime).Max();
                        isSuccess = systemParameterServices
                            .UpdateTimingExecution(custToReactive.Select(c => c.LogTime).Max().ToString(CONVERT_TO_SQL_DATETIME), CTS_SystemParameters.ReactivatedCustomerLastScannedDate);
                    }
                }

                ctscustomerService
                    .InsertReactivedCustomerLog(lastScannedTime, requestTime, (maxLogTime != DateTime.MaxValue) ? maxLogTime : null, (custInfoTime != DateTime.MaxValue) ? custInfoTime : null);

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
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

                        var rescanConfig = new RescanParametersEntity
                        {
                            IsToQueue = false,
                            FunctionId = FunctionId.InactivateNewCustomer,
                            RescanNormalType = RescanNormalType.Auto_InactiveCustomer,
                            InternalBatchSize = batchSize,
                            ServiceName = "InactiveNewCustomer",
                            PushDataExternal = new PushDataToExternalEntity
                            {
                                RoleGroup = RoleGroupType.Member,
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

                    systemParameterServices.UpdateTimingExecution(inactivateCusts.LastCategoryID.ToString(), CTS_SystemParameters.InactiveNewMember_LastCategoryID);
                    systemParameterServices.UpdateTimingExecution(inactivateCusts.LastCustID.ToString(), CTS_SystemParameters.InactiveNewMember_LastCustID);

                    Thread.Sleep(300);

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
            return custClassToMainDBService.PushExternalPackageByCustId(custIDs.Distinct(), new PushDataToExternalEntity
            {
                RoleGroup = RoleGroupType.Member,
                ExternalBatchSize = appSettings.InsertReactivatedCustomersBatchSize,
                IsUpdateDangerToMainDB = true,
                IsWriteLog = appSettings.InsertReactivatedCustomersIsWriteLog,
                ServiceName = serviceName
            });
        }

        #endregion

        #region Private Methods InsertReactivatedCustomers
        private CheckExistCTSArchiveCustModel CheckExistCTSArchiveCustomerAndPushCC(IEnumerable<ReactivatedCustModel> reactivedCusts)
        {
            if (reactivedCusts.IsNullOrEmpty())
            {
                return new CheckExistCTSArchiveCustModel();
            }

            var exitedCTSArchiveCustIDs = ctscustomerService.CheckExistCTSArchiveCustomer(reactivedCusts);

            return new CheckExistCTSArchiveCustModel
            {
                ExistCusts = exitedCTSArchiveCustIDs,
                NotExistCusts = reactivedCusts.Select(x => x.CustID).Except(exitedCTSArchiveCustIDs),
                IsPushCCSuccess = PushDataToExternal(exitedCTSArchiveCustIDs, "InsertReactivatedCustomers")
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        private bool InsertNewCustFromReactivedCust(IEnumerable<long> insertNewCusts)
        {
            if (insertNewCusts.IsNullOrEmpty())
            {
                return true;
            }

            // sleep for latency
            Thread.Sleep(2000);

            var custInfo = ctscustomerService.GetMissingCustInfo(insertNewCusts);

            return ctscustomerService.InsertNewCustomer(false, custInfo, new InsertNewCustRescanEntity
            {
                InternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                ExternalBatchSize = appSettings.TransformCTSCustomerBatchSize,
                IsWriteLog = appSettings.TransformCTSCustomerIsWriteLog,
                IsInsertMissingCust = false,
                InsertVIPURL = appSettings.InsertVIPURL,
                InsertBAURL = appSettings.InsertBAURL,
                serviceName = "ReactivatedCustomer"
            });
        }
        #endregion Private Methods
    }
}