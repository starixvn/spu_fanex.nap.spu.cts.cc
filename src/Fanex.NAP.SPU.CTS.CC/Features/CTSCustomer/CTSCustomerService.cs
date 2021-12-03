namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerService
    {
        LastScannedCustModel GetLastInsertCust();

        UpdateCustomerModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID);

        List<CustomerMainDBModel> GetCustSubFromMainDB(int numOfAccount, Int64 lastScannedCustID);

        bool InsertNewCustomer(UpdateCustomerModel data, bool isWriteLogInfo, int internalBatchSize, string soon88Url, bool isInsertMissingCust);

        LastUpdateValueModel GetLastUpdatedTime();

        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromProductStatusHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        bool IsValidData(TranformCustModel data);

        void UpdateCustomer(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog);

        IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType);

        IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType);

        string GetCustInfoToUpdate(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog, out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks);

        string CompareCustomerInfo(IEnumerable<CustomerMainDBModel> custInfoFromMainDB, IEnumerable<CustomerMainDBModel> custInfoFromCTS, UpdateCustomer sourceType, out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks);

        List<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId);

        List<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs);

        int UpdateUsername2(IEnumerable<GetCustIdModel> customerList);

        IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(List<CustomerMainDBModel> customerList);

        IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<CustomerMainDBModel> CompareCustomerDataForMonitor(IEnumerable<CustomerMainDBModel> customerList);

        DateTime GetLastScanTimeForCustomerMonitor(SystemParameters paraID);

        void UpdateLastScanTimeForCustomerMonitor(DateTime lastScannedDate, SystemParameters paraID);

        void InsertMissingCust(IEnumerable<CustomerMainDBModel> customerList, DateTime scannedDate);
    }

    public class CTSCustomerService : ICTSCustomerService
    {
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;

        private readonly string serviceName = "CTSCustomerService";

        public CTSCustomerService(ICTSCustomerDataAccess dataAccess,
            ISentryClient sentryClient,
            IRescanCustClassificationService rescanClassificationService,
            ISystemParameterDataAccess systemParameterDataAccess)
        {
            this.dataAccess = dataAccess;
            this.rescanClassificationService = rescanClassificationService;
            this.systemParameterDataAccess = systemParameterDataAccess;
            this.sentryClient = sentryClient;
        }

        public LastUpdateValueModel GetLastUpdatedTime()
        {
            return dataAccess.GetLastUpdatedTime();
        }

        public void UpdateCustomer(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog)
        {
            try
            {
                if (custs != null && custs.LastCustId != null && custs.LastUpdatedTime != null)
                {
                    var assCustForRollbacks = Enumerable.Empty<UpdateAssociationCustomerForRollbackModel>();

                    var custInfoToUpdate = GetCustInfoToUpdate(custs, sourceType, isWriteLog, out assCustForRollbacks);

                    UpdateCustomerComplete(custInfoToUpdate, custs.LastUpdatedTime, custs.LastCustId, sourceType);

                    UpdateAssociationCustomerForRollback(assCustForRollbacks);

                    Utilities.LogSentryInfo(serviceName, "UpdateCustomer - done", isWriteLog, sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "UpdateCustomer has an error", custs, ex, sentryClient);
            }
        }

        public string GetCustInfoToUpdate(TranformCustModel custs
            , UpdateCustomer sourceType
            , bool isWriteLog
            , out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            var custInfoToUpdate = JsonConvert.SerializeObject(string.Empty);
            assCustForRollbacks = new List<UpdateAssociationCustomerForRollbackModel>();

            if (!string.IsNullOrWhiteSpace(custs.CustIdList))
            {
                var custInfoFromMainDB = GetCustomerFromMainDBForUpdate(custs.CustIdList, sourceType);
                var custInfoFromCTS = GetCustomerFromCTSForUpdate(custs.CustIdList, sourceType);

                Utilities.LogSentryInfo(serviceName, $"Healing {sourceType} - Get info from MainDB", custInfoFromMainDB, isWriteLog, sentryClient);
                Utilities.LogSentryInfo(serviceName, $"Healing {sourceType} - Get info from CTS", custInfoFromCTS, isWriteLog, sentryClient);

                if (custInfoFromMainDB != null && custInfoFromCTS != null)
                {
                    custInfoToUpdate = CompareCustomerInfo(custInfoFromMainDB, custInfoFromCTS, sourceType, out assCustForRollbacks);
                }
            }

            Utilities.LogSentryInfo(serviceName, $"Healing {sourceType} - Cust to update data:", custInfoToUpdate, isWriteLog, sentryClient);

            return custInfoToUpdate;
        }

        public string CompareCustomerInfo(IEnumerable<CustomerMainDBModel> custInfoFromMainDB
            , IEnumerable<CustomerMainDBModel> custInfoFromCTS
            , UpdateCustomer sourceType
            , out IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            assCustForRollbacks = new List<UpdateAssociationCustomerForRollbackModel>();

            switch (sourceType)
            {
                case Constants.UpdateCustomer.FromCustInfo:
                    {
                        var customerInfo = custInfoFromMainDB
                            .Join(custInfoFromCTS, custMain => custMain.CustID,
                                                   custCTS => custCTS.CustID,
                                                   (custMain, custCTS) => custMain)
                            .Where(c => !custInfoFromCTS.Contains(c, new CustomerInfoEqualityComparer()))
                            .Select(c => new CustInfoModel
                            {
                                CustID = c.CustID,
                                CustStatusID = c.CustStatusID,
                                Danger1 = c.Danger1,
                                Danger2 = c.Danger2,
                                Danger3 = c.Danger3
                            }).ToList();

                        assCustForRollbacks = customerInfo.Select(x => new UpdateAssociationCustomerForRollbackModel
                        {
                            CustID = x.CustID,
                            CustStatusID = x.CustStatusID
                        });

                        return JsonConvert.SerializeObject(customerInfo);
                    }
                case Constants.UpdateCustomer.FromCustomer:
                    {
                        var customerInfo = custInfoFromMainDB
                            .Join(custInfoFromCTS, custMain => custMain.CustID,
                                                   custCTS => custCTS.CustID,
                                                   (custMain, custCTS) => custMain)
                            .Where(c => !custInfoFromCTS.Contains(c, new CustomerEqualityComparer()))
                            .Select(c => new CustomerInfoModel
                            {
                                CustID = c.CustID,
                                UserName = c.UserName,
                                UserName2 = c.UserName2,
                                SiteID = c.SiteID,
                                Site = c.Site,
                                RoleID = c.RoleID,
                                SRecommend = c.SRecommend,
                                MRecommend = c.MRecommend,
                                Recommend = c.Recommend,
                                Currency = c.Currency,
                                CurrencyID = c.CurrencyID,
                                UpdateType = (custInfoFromCTS.Any(f => f.CustID == c.CustID &&
                                                         (!string.Equals(f.UserName, c.UserName)
                                                         || !string.Equals(f.UserName2, c.UserName2)
                                                         || f.SiteID != c.SiteID))) ? 1 : 0
                            }).ToList();

                        return JsonConvert.SerializeObject(customerInfo);
                    }
                case Constants.UpdateCustomer.FromProductStatus:
                    {
                        var customerInfo = custInfoFromMainDB
                            .Join(custInfoFromCTS, custMain => custMain.CustID,
                                                   custCTS => custCTS.CustID,
                                                   (custMain, custCTS) => custMain)
                            .Where(c => !custInfoFromCTS.Contains(c, new CustomerStaticEqualityComparer()))
                            .Select(c => new CustInfoModel
                            {
                                CustID = c.CustID,
                                CustStatusID = c.CustStatusID
                            }).ToList();

                        assCustForRollbacks = customerInfo.Select(x => new UpdateAssociationCustomerForRollbackModel
                        {
                            CustID = x.CustID,
                            CustStatusID = x.CustStatusID
                        });

                        return JsonConvert.SerializeObject(customerInfo);
                    }
                default:
                    return JsonConvert.SerializeObject(string.Empty);
            }
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType)
        {
            return dataAccess.GetCustomerFromMainDBForUpdate(custIds, sourceType);
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType)
        {
            return dataAccess.GetCustomerFromCTSForUpdate(custIds, sourceType);
        }

        public TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            return dataAccess.GetUpdatedCustFromCustInfoHistory(lastUpdatedTime, lastUpdatedCustId, numOfAccount);
        }

        public TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            return dataAccess.GetUpdatedCustFromCustomerHistory(lastUpdatedTime, lastUpdatedCustId, numOfAccount);
        }

        public TranformCustModel GetUpdatedCustFromProductStatusHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            return dataAccess.GetUpdatedCustFromProductStatus(lastUpdatedTime, lastUpdatedCustId, numOfAccount);
        }

        public LastScannedCustModel GetLastInsertCust()
        {
            return dataAccess.GetLastInsertCust();
        }

        public UpdateCustomerModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataAccess.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID);
        }

        public List<CustomerMainDBModel> GetCustSubFromMainDB(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataAccess.GetCustSubFromMainDB(numOfAccount, lastScannedCustID);
        }

        public IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(List<CustomerMainDBModel> customerList)
        {
            var custInfos = new List<InsertCustomerModel>();
            var customerJson = JsonConvert.SerializeObject(customerList);

            try
            {
                custInfos = dataAccess.TransformCustomerFromMainDB(customerJson)?.ToList();
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("InsertNewCustomer", "TransformCustomerFromMainDB exception:", customerJson, ex, sentryClient);
            }

            return custInfos;
        }        

        public void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID, UpdateCustomer sourceType)
        {
            switch (sourceType)
            {
                case Constants.UpdateCustomer.FromCustInfo:
                    {
                        dataAccess.UpdateCustInfoComplete(customers, lastUpdateTime, lastUpdateCustID);
                        break;
                    }
                case Constants.UpdateCustomer.FromCustomer:
                    {
                        dataAccess.UpdateCustomerComplete(customers, lastUpdateTime, lastUpdateCustID);
                        break;
                    }             
                case Constants.UpdateCustomer.FromProductStatus:
                    {
                        dataAccess.UpdateCustomerStatusComplete(customers, lastUpdateTime, lastUpdateCustID);
                        break;
                    }
                default:
                    return;
            }
        }

        public bool InsertNewCustomer(UpdateCustomerModel data, bool isWriteLogInfo, int internalBatchSize, string soon88Url, bool isInsertMissingCust)
        {
            if (data != null && data.CustomerInfos != null && data.CustomerInfos.Any())
            {
                var customers = data.CustomerInfos;
                var lastCustID = data.LastInsertCustID;

                try
                {
                    Utilities.LogSentryInfo(serviceName, "InsertNewCustomer start", customers, isWriteLogInfo, sentryClient);

                    var customersChunk = customers.Distinct().Chunk(internalBatchSize);

                    foreach (var custs in customersChunk)
                    {
                        var custIdList = TransformCustomerFromMainDB(custs.ToList());

                        Utilities.LogSentryInfo(serviceName, "Insert", custs, isWriteLogInfo, sentryClient);

                        UpdateAssociationCustomerForArchive(custIdList);

                        RescanCCAndNotifyCompleteForNewCustomer(custIdList, soon88Url, internalBatchSize, isWriteLogInfo, lastCustID, isInsertMissingCust);
                    }

                    Utilities.LogSentryInfo(serviceName, "InsertNewCustomer start", isWriteLogInfo, sentryClient);
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, "InsertNewCustomer throws exception", customers, ex, sentryClient);

                    return false;
                }
            }

            return true;
        }

        public void RescanCCAndNotifyCompleteForNewCustomer(IEnumerable<InsertCustomerModel> custIdList, string soon88Url, int internalBatchSize, bool isWriteLogInfo, long? lastCustID, bool isInsertMissingCust)
        {
            if (custIdList != null && custIdList.Any())
            {
                var isSuccess = RescanCCForNewCustomer(custIdList, soon88Url, internalBatchSize, isWriteLogInfo);

                if (isSuccess)
                {
                    NotifyCompleteForNewCustomer(custIdList, isWriteLogInfo, lastCustID, isInsertMissingCust);
                }
            }
        }

        public bool IsValidData(TranformCustModel data)
        {
            if (data != null && data.LastCustId != null && data.LastUpdatedTime != null)
            {
                return true;
            }

            return false;
        }

        public List<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId)
        {
            return dataAccess.GetCustWithNullUserName2(rowSize, preCTSCustId).ToList();
        }

        public List<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs)
        {
            return dataAccess.GetCustWithHavingUserName2(custs).ToList();
        }

        public int UpdateUsername2(IEnumerable<GetCustIdModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);

            return dataAccess.UpdateUsername2(customerJson);
        }

        public DateTime GetLastScanTimeForCustomerMonitor(SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            var minDateInMainDB = new DateTime(2003, 1, 1);
            return lastScannedDate != null ? DateTime.Parse(lastScannedDate?.ParameterValue) : minDateInMainDB;
        }

        public void UpdateLastScanTimeForCustomerMonitor(DateTime lastScannedDate , SystemParameters paraID)
        {
            systemParameterDataAccess.UpdateTimingExecution(lastScannedDate.ToString("yyyy-MM-dd HH:mm:ss"), paraID);
        }

        public IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate)
        {
            return dataAccess.GetCustInfoFromMainDBForMonitor(fromDate, toDate);
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate)
        {
            return dataAccess.GetCustSubInfoFromMainDBForMonitor(fromDate, toDate);
        }

        public IEnumerable<CustomerMainDBModel> CompareCustomerDataForMonitor(IEnumerable<CustomerMainDBModel> customerList)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);
            return dataAccess.CompareCustomerDataForMonitor(customerJson);
        }

        public void InsertMissingCust(IEnumerable<CustomerMainDBModel> customerList, DateTime scannedDate)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);
            dataAccess.InsertMissingCust(customerJson, scannedDate);
        }

        private void NotifyCompleteForNewCustomer(IEnumerable<InsertCustomerModel> custIdList, bool isWriteLogInfo, long? lastCustID, bool isInsertMissingCust)
        {
            try
            {
                dataAccess.ReTransformAccount(JsonConvert.SerializeObject(custIdList), lastCustID, isInsertMissingCust);

                Utilities.LogSentryInfo(serviceName, "InsertNewCustomer - Insert complete", custIdList, isWriteLogInfo, sentryClient);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertNewCustomer - RReTransformAccount throws exception", custIdList, ex, sentryClient);
            }
        }

        private bool RescanCCForNewCustomer(IEnumerable<InsertCustomerModel> custIdList, string soon88Url, int internalBatchSize, bool isWriteLogInfo)
        {
            var result = false;

            try
            {
                var custToRescan = custIdList.Select(c => c.CustID).Distinct();

                var rescanParam = new RescanParametersEntity
                {
                    CustIds = custToRescan,
                    FunctionId = FunctionId.InsertNewCustJob,
                    IsCheckPAFromNAP = true,
                    IsPushCCToSoon88 = true,
                    IsUpdateDangerToMainDB = true,
                    DangerLevelType = DangerLevel.ABI,
                    ServiceName = serviceName,
                    FeedSoon88API = soon88Url,
                    IsWriteLog = isWriteLogInfo,
                    InternalBatchSize = internalBatchSize,
                    ExternalBatchSize = internalBatchSize
                };
                
                result = rescanClassificationService.RescanCustomerClassify(rescanParam);

                Utilities.LogSentryInfo(serviceName, "InsertNewCustomer - RescanCustomerClassify done", custToRescan, isWriteLogInfo, sentryClient);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertNewCustomer - RescanCustomerClassify done", custIdList, ex, sentryClient);
            }

            return result;
        }

        private void UpdateAssociationCustomerForArchive(IEnumerable<InsertCustomerModel> customerList)
        {
            if (customerList != null && customerList.Any())
            {
                try
                {
                    var custList = customerList.Select(x => new UpdateAssociationCustomerForArchiveModel
                    {
                        CustID = x.CustID,
                        CTSCustID = x.CTSCustID,
                        CustSubID = x.CustSubID,
                        RoleID = x.RoleID                        
                    });

                    var custInfoJson = JsonConvert.SerializeObject(custList);

                    var updateResult = dataAccess.UpdateAssociationCustomerForArchive(custInfoJson);
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, "UpdateAssociationCustomerForArchive throws exception", customerList, ex, sentryClient);
                }
            }
        }

        private void UpdateAssociationCustomerForRollback(IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                try
                {
                    var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                    var updateResult = dataAccess.UpdateAssociationCustomerForRollback(custInfoJson);
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, "UpdateAssociationCustomerForRollback throws exception", assCustForRollbacks, ex, sentryClient);
                }
            }
        }
    }
}