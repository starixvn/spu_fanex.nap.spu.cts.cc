namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerCategory;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
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

        InsertCustomerViewModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(int numOfAccount, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(string custIds);

        bool InsertNewCustomer(InsertCustomerViewModel data, InsertNewCustRescanEntity paras);

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

        IEnumerable<InsertNewCustomerResultModel> InsertNewCTSCustomer(IEnumerable<CustomerMainDBModel> customers);

        IEnumerable<CustomerMainDBModel> GetCustInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<CustomerMainDBModel> GetCustSubInfoFromMainDBForMonitor(DateTime fromDate, DateTime toDate);

        IEnumerable<CustomerMainDBModel> CompareCustomerDataForMonitor(IEnumerable<CustomerMainDBModel> customerList);

        DateTime GetLastScanTimeForCustomerMonitor(SystemParameters paraID);

        void UpdateLastScanTimeForCustomerMonitor(DateTime lastScannedDate, SystemParameters paraID);

        void InsertMissingCust(IEnumerable<CustomerMainDBModel> customerList, DateTime scannedDate);

        IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime? lastScannedDate);

        InactivateNewCustomer GetInactivateNewCustomers();
    }

    public class CTSCustomerService : ICTSCustomerService
    {
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly ICustomerCategoryService customerCategoryService;
        private readonly ICustClassToMainDBService custClassToMainDBService;
        private readonly IAntiFraudCenterServices antiFraudCenterServices;
        private readonly ISystemParameterDataAccess systemParameterDataAccess;
        private readonly string serviceName = "CTSCustomerService";

        public CTSCustomerService(ICTSCustomerDataAccess dataAccess,
            ISentryClient sentryClient,
            IRescanCustClassificationService rescanClassificationService,
            ICustomerCategoryService customerCategoryService,
            ICustClassToMainDBService custClassToMainDBService,
            IAntiFraudCenterServices antiFraudCenterServices,
            ISystemParameterDataAccess systemParameterDataAccess)
        {
            this.dataAccess = dataAccess;
            this.rescanClassificationService = rescanClassificationService;
            this.customerCategoryService = customerCategoryService;
            this.custClassToMainDBService = custClassToMainDBService;
            this.antiFraudCenterServices = antiFraudCenterServices;
            this.systemParameterDataAccess = systemParameterDataAccess;
            this.sentryClient = sentryClient;
        }

        public LastUpdateValueModel GetLastUpdatedTime()
        {
            return dataAccess.GetLastUpdatedTime();
        }

        #region Insert New Customers

        public LastScannedCustModel GetLastInsertCust()
        {
            return dataAccess.GetLastInsertCust();
        }

        public InsertCustomerViewModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataAccess.GetCustomerFromMainDBForInsertNew(numOfAccount, lastScannedCustID);
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(int numOfAccount, Int64 lastScannedCustID)
        {
            return dataAccess.GetCustSubFromMainDB(numOfAccount, lastScannedCustID);
        }

        public bool InsertNewCustomer(InsertCustomerViewModel data, InsertNewCustRescanEntity paras)
        {
            try
            {
                if (data != null && data.CustomerInfos != null && data.CustomerInfos.Any())
                {
                    var chunkCusts = data.CustomerInfos.Distinct().Chunk(paras.InternalBatchSize);

                    foreach (var batchCust in chunkCusts)
                    {
                        var customers = InsertNewCTSCustomer(batchCust.ToList());

                        if (customers != null && customers.Any())
                        {
                            UpdateAssociationCustomerForArchive(customers);

                            UpdateComleteAndRetranCustomer(customers, data.LastInsertCustID, paras.IsInsertMissingCust);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertNewCustomer throws exception", data.CustomerInfos, ex, sentryClient);

                return false;
            }

            return true;
        }

        public IEnumerable<InsertNewCustomerResultModel> InsertNewCTSCustomer(IEnumerable<CustomerMainDBModel> customers)
        {
            if (customers != null && customers.Any())
            {
                return dataAccess.InsertNewCTSCustomer(customers);
            }

            return Enumerable.Empty<InsertNewCustomerResultModel>();
        }

        public void InsertMissingCust(IEnumerable<CustomerMainDBModel> customerList, DateTime scannedDate)
        {
            var customerJson = JsonConvert.SerializeObject(customerList);
            dataAccess.InsertMissingCust(customerJson, scannedDate);
        }

        private void UpdateComleteAndRetranCustomer(IEnumerable<InsertNewCustomerResultModel> custIdList, long? lastCustID, bool isInsertMissingCust)
        {
            if (custIdList != null && custIdList.Any())
            {
                dataAccess.UpdateCompleteAndRetranCustomer(JsonConvert.SerializeObject(custIdList), lastCustID, isInsertMissingCust);
            }
        }

        private bool RescanCCForNewCustomer(IEnumerable<InsertNewCustomerResultModel> custIdList, InsertNewCustRescanEntity paras)
        {
            var result = false;

            try
            {
                var custToRescan = custIdList.Select(c => c.CustID).Distinct();

                var rescanParam = new RescanParametersEntity
                {
                    CustIds = custToRescan,
                    FunctionId = FunctionId.InsertNewCustJob,
                    //IsCheckPAFromNAP = true,  //TODO
                    IsPushCCToSoon88 = true,
                    IsUpdateDangerToMainDB = true,
                    ServiceName = serviceName,
                    FeedSoon88API = paras.FeedSoon88API,
                    IsWriteLog = paras.IsWriteLog,
                    InternalBatchSize = paras.InternalBatchSize,
                    ExternalBatchSize = paras.ExternalBatchSize,
                    IsSyncAFCData = paras.IsSyncAFCData,
                    AFCInsertAccountURL = paras.AFCInsertAccountURL
                };

                result = rescanClassificationService.RescanCustomerClassification(rescanParam);

                Utilities.LogSentryInfo(serviceName, "InsertNewCustomer - RescanCustomerClassify done", custToRescan, paras.IsWriteLog, sentryClient);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertNewCustomer - RescanCustomerClassify done", custIdList, ex, sentryClient);
            }

            return result;
        }

        private void UpdateAssociationCustomerForArchive(IEnumerable<InsertNewCustomerResultModel> customerList)
        {
            if (customerList != null && customerList.Any())
            {
                var custList = customerList.Select(x => new UpdateAssociationCustomerForArchiveModel
                {
                    CustID = x.CustID,
                    CTSCustID = x.CTSCustID,
                    CustSubID = x.CustSubID,
                    RoleID = x.RoleID
                });

                var custInfoJson = JsonConvert.SerializeObject(custList);

                dataAccess.UpdateAssociationCustomerForArchive(custInfoJson);
            }
        }

        public DateTime GetLastScanTimeForCustomerMonitor(SystemParameters paraID)
        {
            var lastScannedDate = systemParameterDataAccess.GetPreviousTimingExecution(paraID);
            var minDateInMainDB = new DateTime(2003, 1, 1);
            return lastScannedDate != null ? DateTime.Parse(lastScannedDate?.ParameterValue) : minDateInMainDB;
        }

        public void UpdateLastScanTimeForCustomerMonitor(DateTime lastScannedDate, SystemParameters paraID)
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

        #endregion Insert New Customers

        #region Update Customers Info

        public void UpdateCustomer(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog)
        {
            try
            {
                if (custs != null && custs.LastCustId != null && custs.LastUpdatedTime != null)
                {
                    var assCustForRollbacks = Enumerable.Empty<UpdateAssociationCustomerForRollbackModel>();

                    var custInfoToUpdate = GetCustInfoToUpdate(custs, sourceType, isWriteLog, out assCustForRollbacks);

                    UpdateAssociationCustomerForRollback(assCustForRollbacks);

                    UpdateCustomerComplete(custInfoToUpdate, custs.LastUpdatedTime, custs.LastCustId, sourceType);
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
            return dataAccess.GetCustomerFromMainDBForUpdate(custIds, sourceType) ?? Enumerable.Empty<CustomerMainDBModel>();
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

        private void UpdateAssociationCustomerForRollback(IEnumerable<UpdateAssociationCustomerForRollbackModel> assCustForRollbacks)
        {
            if (assCustForRollbacks != null && assCustForRollbacks.Any())
            {
                var custInfoJson = JsonConvert.SerializeObject(assCustForRollbacks);

                dataAccess.UpdateAssociationCustomerForRollback(custInfoJson);
            }
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

        #endregion Update Customers Info

        #region Update UserName2

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

        #endregion Update UserName2

        #region Insert Reactivated Customers

        public IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime? lastScannedDate)
        {
            if (lastScannedDate != null)
            {
                return dataAccess.GetReactivatedCustomers(lastScannedDate.Value);
            }

            return Enumerable.Empty<ReactivatedCustModel>();
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(string custIds)
        {
            return dataAccess.GetCustSubFromMainDB(custIds) ?? Enumerable.Empty<CustomerMainDBModel>();
        }

        #endregion Insert Reactivated Customers

        #region Inactivate New Customers

        public InactivateNewCustomer GetInactivateNewCustomers()
        {
            return dataAccess.GetInactivateNewCustomers();
        }

        #endregion Inactivate New Customers
    }
}