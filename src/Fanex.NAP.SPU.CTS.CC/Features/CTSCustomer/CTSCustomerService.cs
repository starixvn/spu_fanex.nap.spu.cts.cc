namespace Fanex.NAP.SPU.CTS.Features.CTSCustomer
{
    using Fanex.NAP.SPU.CTS.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerService
    {
        LastScannedCustModel GetLastInsertCust();

        UpdateCustomerModel GetCustomerFromMainDBForInsertNew(int numOfAccount, Int64 lastScannedCustID);

        List<CustomerMainDBModel> GetCustSubFromMainDB(int numOfAccount, Int64 lastScannedCustID);

        bool InsertNewCustomer(UpdateCustomerModel data, bool isWriteLogInfo, int internalBatchSize, string soon88Url);

        LastUpdateValueModel GetLastUpdatedTime();

        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        bool IsValidData(TranformCustModel custFromCustInfo, TranformCustModel custFromCustomer);

        void UpdateCustomer(TranformCustModel custs, int sourceType, bool isWriteLog);

        IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType);

        IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType);

        string GetCustInfoToUpdate(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog);

        string CompareCustomerInfo(IEnumerable<CustomerMainDBModel> custInfoFromMainDB, IEnumerable<CustomerMainDBModel> custInfoFromCTS, UpdateCustomer sourceType);

        List<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId);

        List<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs);

        int UpdateUsername2(IEnumerable<GetCustIdModel> customerList);

        IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(List<CustomerMainDBModel> customerList);
    }

    public class CTSCustomerService : ICTSCustomerService
    {
        private readonly ICTSCustomerDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        private readonly IRescanCustClassificationService rescanClassificationService;
        private readonly string serviceName = "CTSCustomerService";

        public CTSCustomerService(ICTSCustomerDataAccess dataAccess,
            ISentryClient sentryClient,
            IRescanCustClassificationService rescanClassificationService)
        {
            this.dataAccess = dataAccess;
            this.rescanClassificationService = rescanClassificationService;
            this.sentryClient = sentryClient;
        }

        public LastUpdateValueModel GetLastUpdatedTime()
        {
            return dataAccess.GetLastUpdatedTime();
        }

        public void UpdateCustomer(TranformCustModel custs, int sourceType, bool isWriteLog)
        {
            try
            {
                if (custs != null && custs.LastCustId != null && custs.LastUpdatedTime != null)
                {
                    var updateCustomerType = (UpdateCustomer)sourceType;

                    var custInfoToUpdate = GetCustInfoToUpdate(custs, updateCustomerType, isWriteLog);

                    UpdateCustomerComplete(custInfoToUpdate, custs.LastUpdatedTime, custs.LastCustId, updateCustomerType);

                    Utilities.LogSentryInfo(serviceName, "Healing - done", isWriteLog, sentryClient);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "UpdateCustomer has an error", custs, ex, sentryClient);
            }
        }

        public string GetCustInfoToUpdate(TranformCustModel custs, UpdateCustomer sourceType, bool isWriteLog)
        {
            var custInfoToUpdate = JsonConvert.SerializeObject(string.Empty);

            if (!string.IsNullOrWhiteSpace(custs.CustIdList))
            {
                var custInfoFromMainDB = GetCustomerFromMainDBForUpdate(custs.CustIdList, sourceType);
                var custInfoFromCTS = GetCustomerFromCTSForUpdate(custs.CustIdList, sourceType);

                var messageLog = $"Healing - Get {((sourceType == Constants.UpdateCustomer.FromCustomer) ? "Customer History" : "CustInfo")} from MainDB";
                var messageLog2 = $"Healing - Get {((sourceType == Constants.UpdateCustomer.FromCustomer) ? "Customer History" : "CustInfo")} from CTS";

                Utilities.LogSentryInfo(serviceName, messageLog, custInfoFromMainDB, isWriteLog, sentryClient);
                Utilities.LogSentryInfo(serviceName, messageLog2, custInfoFromCTS, isWriteLog, sentryClient);

                if (custInfoFromMainDB != null && custInfoFromCTS != null)
                {
                    custInfoToUpdate = CompareCustomerInfo(custInfoFromMainDB, custInfoFromCTS, sourceType);
                }
            }

            Utilities.LogSentryInfo(serviceName, "Healing - Cust to update data:", custInfoToUpdate, isWriteLog, sentryClient);

            return custInfoToUpdate;
        }

        public string CompareCustomerInfo(IEnumerable<CustomerMainDBModel> custInfoFromMainDB, IEnumerable<CustomerMainDBModel> custInfoFromCTS, UpdateCustomer sourceType)
        {
            if (sourceType == Constants.UpdateCustomer.FromCustInfo)
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

                return JsonConvert.SerializeObject(customerInfo);
            }
            else
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
                                             || f.SiteID != c.SiteID)) == true) ? 1 : 0
                    }).ToList();

                return JsonConvert.SerializeObject(customerInfo);
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
            if (sourceType == Constants.UpdateCustomer.FromCustomer)
            {
                dataAccess.UpdateCustomerComplete(customers, lastUpdateTime, lastUpdateCustID);
            }
            else
            {
                dataAccess.UpdateCustInfoComplete(customers, lastUpdateTime, lastUpdateCustID);
            }
        }

        public bool InsertNewCustomer(UpdateCustomerModel data, bool isWriteLogInfo, int internalBatchSize, string soon88Url)
        {
            if (data is not null && data.CustomerInfos != null && data.CustomerInfos.Any())
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

                        RescanCCAndNotifyCompleteForNewCustomer(custIdList, soon88Url, internalBatchSize, isWriteLogInfo, lastCustID);
                    }

                    Utilities.LogSentryInfo(serviceName, "InsertNewCustomer start", isWriteLogInfo, sentryClient);

                    return true;
                }
                catch (Exception ex)
                {
                    Utilities.LogSentryError(serviceName, "InsertNewCustomer throws exception", customers, ex, sentryClient);
                }
            }

            return false;
        }

        public void RescanCCAndNotifyCompleteForNewCustomer(IEnumerable<InsertCustomerModel> custIdList, string soon88Url, int internalBatchSize, bool isWriteLogInfo, long? lastCustID)
        {
            if (custIdList != null && custIdList.Any())
            {
                var isSuccess = RescanCCForNewCustomer(custIdList, soon88Url, internalBatchSize, isWriteLogInfo);

                if (isSuccess)
                {
                    NotifyCompleteForNewCustomer(custIdList, isWriteLogInfo, lastCustID);
                }
            }
        }

        public bool IsValidData(TranformCustModel custFromCustInfo, TranformCustModel custFromCustomer)
        {
            if ((custFromCustInfo != null && custFromCustInfo.LastCustId != null && custFromCustInfo.LastUpdatedTime != null)
                || (custFromCustomer != null && custFromCustomer.LastCustId != null && custFromCustomer.LastUpdatedTime != null))
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

        private void NotifyCompleteForNewCustomer(IEnumerable<InsertCustomerModel> custIdList, bool isWriteLogInfo, long? lastCustID)
        {
            try
            {
                dataAccess.ReTransformAccount(JsonConvert.SerializeObject(custIdList), lastCustID);

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
                    ServiceName = serviceName,
                    FeedSoon88API = soon88Url,
                    IsWriteLog = isWriteLogInfo,
                    InternalBatchSize = internalBatchSize,
                    ExternalBatchSize = internalBatchSize
                };
                rescanClassificationService.RescanCustomerClassify(rescanParam);

                result = true;
                Utilities.LogSentryInfo(serviceName, "InsertNewCustomer - RescanCustomerClassify done", custToRescan, isWriteLogInfo, sentryClient);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, "InsertNewCustomer - RescanCustomerClassify done", custIdList, ex, sentryClient);
            }

            return result;
        }
    }
}