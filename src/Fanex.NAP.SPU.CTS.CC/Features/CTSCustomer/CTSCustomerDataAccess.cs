namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.InsertLatencyCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.InsertMissingCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria.ScanLatencyCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Entities.ScanLatencyCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.Accounts.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.AccessControl;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerDataAccess
    {
        LastScannedCustModel GetLastInsertCust();

        InsertCustomerModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByLastCustSub(int quantityPerBatch, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByCustIDs(string custIds);

        IEnumerable<InsertNewCustomerResultModel> InsertNewCTSCustomer(IEnumerable<CustomerMainDBEntity> customerInfo, bool isNewCustomer);

        void UpdateAssociationCustomerForArchive(string custInfoJson);

        void UpdateAssociationCustomerForRollback(string custInfoJson);

        void UpdateCustInfoComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        void UpdateCustomerStatusComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        LastUpdateValueModel GetLastUpdatedTime();

        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime);

        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime);

        TranformCustModel GetUpdatedCustFromProductStatus(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime);

        InsertCustomerModel CheckCustomerInfoFromMainDB(string custIds);

        IEnumerable<CustomerMainDBEntity> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType);

        IEnumerable<CustomerMainDBEntity> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType);

        #region Insert Missing Customers

        public IEnumerable<MissingCustomerModel> GetMissingCustomers();

        void InsertMissingCustomerLatencyToQueue(string custIds);

        void CompleteMissingCustomer(string resolvedIDs);

        #endregion

        #region UserName2

        ScannedCustModel GetCustWithNullUserName2(int rowSize, UInt64 lastCTSCustId, bool isCustSub);

        IEnumerable<CustNullUserName2Model> GetCustWithHavingUserName2(IEnumerable<CustNullUserName2Model> custs, bool isCustSub);

        void UpdateUsername2(IEnumerable<CustNullUserName2Model> custInfo);

        #endregion

        void UpdateCompleteAndRetranCustomer(string jsonData, long? lastCustID, bool isInsertMissingCust);

        IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime lastScannedDate, DateTime? nextScannedDate);

        IEnumerable<long> CheckExistCTSArchiveCustomer(string custJson);

        InactivateNewCustomer GetInactivateNewCustomers();

        void InsertReactivedCustomerLog(DateTime? lastScanTime, DateTime? requestTime, DateTime? maxLogTime, DateTime? custInfoTime);

        #region ScanLatencyCustomer
        int GetLastScannedCustID(int fromCustID, int toCustID, int scannedTime);

        ScanLatencyCustomerEntity ScanLatencyCustomer(int fromCustID, int toCustID, int batchSize);

        IEnumerable<int> CheckMissingCustomer(string custIDs);
        #endregion

        #region InsertLatencyCustomer
        IEnumerable<int> GetLatencyCustomerFromQueue(int batchSize);

        void CompleteLatencyCustomerFromQueue(string custIds);
        #endregion
    }

    public class CTSCustomerDataAccess : ICTSCustomerDataAccess
    {
        private readonly IDynamicRepository _repository;

        public CTSCustomerDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public LastUpdateValueModel GetLastUpdatedTime()
        {
            var criteria = new GetLastUpdatedTimeCriteria();

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var result = db.QueryMultiple(criteria);
            var LastUpdateTimeCustInfo = result.Read<DateTime>().FirstOrDefault();
            var LastUpdateCustIDCustInfo = result.Read<Int64>().FirstOrDefault();
            var LastUpdateTimeCustomer = result.Read<DateTime>().FirstOrDefault();
            var LastUpdateCustIDCustomer = result.Read<Int64>().FirstOrDefault();
            var LastUpdateTimeCustomerStatus = result.Read<DateTime>().FirstOrDefault();
            var LastUpdateCustIDCustomerStatus = result.Read<Int64>().FirstOrDefault();

            return new LastUpdateValueModel
            {
                LastUpdateCustIDCustInfo = LastUpdateCustIDCustInfo,
                LastUpdateTimeCustInfo = LastUpdateTimeCustInfo,
                LastUpdateCustIDCustomer = LastUpdateCustIDCustomer,
                LastUpdateTimeCustomer = LastUpdateTimeCustomer,
                LastUpdateCustIDCustProductStatus = LastUpdateCustIDCustomerStatus,
                LastUpdateTimeCustProductStatus = LastUpdateTimeCustomerStatus
            };
        }

        #region Insert New Customers

        public LastScannedCustModel GetLastInsertCust()
        {
            var criteria = new GetLastInsertCustCriteria();

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var result = db.QueryMultiple(criteria);
            var LastCustID = result.Read<Int64>().FirstOrDefault();
            var LastCustSubID = result.Read<Int64>().FirstOrDefault();

            return new LastScannedCustModel
            {
                LastCustID = LastCustID,
                LastCustSubID = LastCustSubID
            };
        }

        public InsertCustomerModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustomersFromMainDbCriteria
            {
                LastCustId = lastScannedCustID,
                RowSize = quantityPerBatch,
            };

            (var customers, var missingCustIds, var lastCustID) = _repository.FetchMultiple<CustomerMainDBEntity, int, Int64>(criteria);

            return new InsertCustomerModel
            {
                CustomerInfos = customers?.ToList() ?? new List<CustomerMainDBEntity>(),
                MissingCustIds = missingCustIds?.ToList() ?? new List<int>(),
                LastInsertCustID = lastCustID?.FirstOrDefault()
            };
        }

        public IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByLastCustSub(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustSubFromMainDbCriteria
            {
                LastCustSubId = lastScannedCustID,
                RowSize = quantityPerBatch,
                QueryType = (int)QueryType.GetByRange,
                CustIdList = ""
            };

            return _repository.Fetch<CustomerMainDBEntity>(criteria) ?? Enumerable.Empty<CustomerMainDBEntity>();
        }

        public IEnumerable<InsertNewCustomerResultModel> InsertNewCTSCustomer(IEnumerable<CustomerMainDBEntity> customerInfo, bool isNewCustomer)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var criteria = new NewCTSCustomerInsertCriteria
                {
                    CustomerJson = JsonConvert.SerializeObject(customerInfo),
                    IsNewCustomer = isNewCustomer
                };

                return _repository.Fetch<InsertNewCustomerResultModel>(criteria) ?? Enumerable.Empty<InsertNewCustomerResultModel>();
            }

            return Enumerable.Empty<InsertNewCustomerResultModel>();
        }

        public void UpdateAssociationCustomerForArchive(string custInfoJson)
        {
            var criteria = new UpdateAssociationCustomerForArchiveCriteria
            {
                CustInfoJson = custInfoJson
            };

            _repository.Execute(criteria);
        }

        public void UpdateAssociationCustomerForRollback(string custInfoJson)
        {
            var criteria = new UpdateAssociationCustomerForRollbackCriteria
            {
                CustInfoJson = custInfoJson
            };

            _repository.Execute(criteria);
        }

        public void UpdateCompleteAndRetranCustomer(string jsonData, long? lastCustID, bool isInsertMissingCust)
        {
            var criteria = new UpdateCompleteAndRetranCustomerCriteria
            {
                CustomerInfo = jsonData,
                LastInsertCustID = lastCustID,
                IsInsertMissingCust = isInsertMissingCust
            };

            _repository.Execute(criteria);
        }

        #endregion Insert New Customers

        #region Insert Missing Customer

        public IEnumerable<MissingCustomerModel> GetMissingCustomers()
        {
            var criteria = new GetMissingCustomerCriteria();

            return _repository.Fetch<MissingCustomerModel>(criteria);
        }

        public void InsertMissingCustomerLatencyToQueue(string custIds)
        {
            var criteria = new InsertMissingCustomerLatencyToQueueCriteria
            {
                CustIDs = custIds
            };

            _repository.Execute(criteria);
        }

        public void CompleteMissingCustomer(string resolvedIDs)
        {
            var criteria = new CompleteMissingCustomerCriteria
            {
                ResolvedIDs = resolvedIDs
            };

            _repository.Execute(criteria);
        }

        #endregion

        #region Update Customers Info

        public TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime)
        {
            var criteria = new GetUpdatedCustFromCustInfoHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
                WaitTime = waitTime
            };

            var (firstResultSet, secondResultSet) = _repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel
            {
                LastUpdatedTime = firstResultSet.SingleOrDefault()?.LastUpdatedTime,
                LastCustId = firstResultSet.SingleOrDefault()?.LastCustId,
                CustIdList = secondResultSet.SingleOrDefault()
            };
        }

        public TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime)
        {
            var criteria = new GetUpdatedCustFromCustomerHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
                WaitTime = waitTime
            };

            var (firstResultSet, secondResultSet) = _repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel
            {
                LastUpdatedTime = firstResultSet.SingleOrDefault()?.LastUpdatedTime,
                LastCustId = firstResultSet.SingleOrDefault()?.LastCustId,
                CustIdList = secondResultSet.SingleOrDefault()
            };
        }

        public TranformCustModel GetUpdatedCustFromProductStatus(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime)
        {
            var criteria = new GetUpdatedCustFromProductStatusHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
                WaitTime = waitTime
            };

            var (firstResultSet, secondResultSet) = _repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel
            {
                LastUpdatedTime = firstResultSet.SingleOrDefault()?.LastUpdatedTime,
                LastCustId = firstResultSet.SingleOrDefault()?.LastCustId,
                CustIdList = secondResultSet.SingleOrDefault()
            };
        }

        public void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var criteria = new UpdateCustomerCompleteCriteria
            {
                CustomerInfo = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            _repository.Execute(criteria);
        }

        public void UpdateCustInfoComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var criteria = new UpdateCustInfoCompleteCriteria
            {
                CustInfo = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            _repository.Execute(criteria);
        }

        public void UpdateCustomerStatusComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var criteria = new UpdateCustomerStatusCompleteCriteria
            {
                CustProductStatus = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            _repository.Execute(criteria);
        }

        public InsertCustomerModel CheckCustomerInfoFromMainDB(string custIds)
        {
            var criteria = new GetCustomerInfoFromMainDBCriteria
            {
                CustIds = custIds,
                SourceType = (int)UpdateCustomer.ForNewCustomer
            };

            (var custInfos, var missingCusts) = _repository.FetchMultiple<CustomerMainDBEntity, int>(criteria);

            return new InsertCustomerModel
            {
                CustomerInfos = custInfos?.ToList() ?? new List<CustomerMainDBEntity>(),
                MissingCustIds = missingCusts?.ToList() ?? new List<int>()
            };
        }

        public IEnumerable<CustomerMainDBEntity> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType)
        {
            var criteria = new GetCustomerInfoFromMainDBCriteria
            {
                CustIds = custIds,
                SourceType = (int)sourceType
            };

            var custInfos = _repository.Fetch<CustomerMainDBEntity>(criteria);

            return custInfos;
        }

        public IEnumerable<CustomerMainDBEntity> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType)
        {
            var criteria = new GetCustomerInfoFromCTSCriteria
            {
                CustIds = custIds,
                SourceType = (int)sourceType
            };

            var custInfos = _repository.Fetch<CustomerMainDBEntity>(criteria);

            return custInfos;
        }

        #endregion Update Customers Info

        #region Update UserName2

        public ScannedCustModel GetCustWithNullUserName2(int rowSize, UInt64 lastCTSCustId, bool isCustSub)
        {
            var criteria = new GetCustomersWithNullUsername2Criteria
            {
                Size = rowSize,
                PreCTSCustID = lastCTSCustId,
                IsCustSub = isCustSub
            };

            var data = _repository.Fetch<CustNullUserName2Model>(criteria);

            return new ScannedCustModel
            {
                CustList = data ?? Enumerable.Empty<CustNullUserName2Model>(),
                LastCTSCustID = criteria.LastCTSCustID
            };
        }

        public IEnumerable<CustNullUserName2Model> GetCustWithHavingUserName2(IEnumerable<CustNullUserName2Model> custs, bool isCustSub)
        {
            var criteria = new GetCustomersWithHavingUsername2Criteria
            {
                CustomerXML = custs.ToXmlString(),
                IsCustSub = isCustSub
            };

            return _repository.Fetch<CustNullUserName2Model>(criteria);
        }

        public void UpdateUsername2(IEnumerable<CustNullUserName2Model> custInfo)
        {
            var criteria = new UpdateUsername2Criteria
            {
                CustomerJson = JsonConvert.SerializeObject(custInfo)
            };

            _repository.Execute(criteria);
        }

        #endregion Update UserName2

        #region Insert Reactivated Customers

        public IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime lastScannedDate, DateTime? nextScannedDate)
        {
            var criteria = new ReactivatedCustomerGetCriteria
            {
                LastScannedDate = lastScannedDate,
                NextScannedDate = nextScannedDate
            };

            return _repository.Fetch<ReactivatedCustModel>(criteria) ?? Enumerable.Empty<ReactivatedCustModel>();
        }

        public IEnumerable<long> CheckExistCTSArchiveCustomer(string custJson)
        {
            var criteria = new CheckExistCTSArchiveCustomerCriteria
            {
                CustJson = custJson
            };

            return _repository.Fetch<long>(criteria) ?? Enumerable.Empty<long>();
        }

        public IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByCustIDs(string custIds)
        {
            var criteria = new GetCustSubFromMainDbCriteria
            {
                CustIdList = custIds,
                QueryType = (int)QueryType.GetByCustId
            };

            return _repository.Fetch<CustomerMainDBEntity>(criteria) ?? Enumerable.Empty<CustomerMainDBEntity>();
        }

        public void InsertReactivedCustomerLog(DateTime? lastScanTime, DateTime? requestTime, DateTime? maxLogTime, DateTime? custInfoTime)
        {
            var criteria = new ReactivedCustomerLogInsertCriteria
            {
                LastScanTime = lastScanTime,
                RequestTime = requestTime,
                MaxLogTime = maxLogTime,
                CustInfoTime = custInfoTime
            };

            _repository.Execute(criteria);
        }

        #endregion Insert Reactivated Customers

        #region Inactive New Customers

        public InactivateNewCustomer GetInactivateNewCustomers()
        {
            var data = new InactivateNewCustomer();
            var criteria = new GetInactivateNewCustomerCriteria();

            data.CustInfos = _repository.Fetch<RescanCategoryDailyModel>(criteria) ?? Enumerable.Empty<RescanCategoryDailyModel>();
            data.LastCustID = criteria.LastCustID;
            data.LastCategoryID = criteria.LastCategoryID;

            return data;
        }

        #endregion Inactive New Customers

        #region ScanLatencyCustomer
        public int GetLastScannedCustID(int fromCustID, int toCustID, int scannedTime)
        {
            var criteria = new GetLastScannedCustIDCriteria
            {
                FromCustID = fromCustID,
                ToCustID = toCustID,
                ScannedTime = scannedTime
            };

            return _repository.Fetch<int>(criteria).First();
        }

        public ScanLatencyCustomerEntity ScanLatencyCustomer(int fromCustID, int toCustID, int batchSize)
        {
            var criteria = new ScanLatencyCustomerCriteria
            {
                FromCustID = fromCustID,
                ToCustID = toCustID,
                BatchSize = batchSize
            };

            var result = _repository.Fetch<int>(criteria);

            return new ScanLatencyCustomerEntity
            {
                CustIDs = result,
                NextScannedCustID = result.Max()
            };
        }

        public IEnumerable<int> CheckMissingCustomer(string custIDs)
        {
            var criteria = new CheckMissingCustCriteria
            {
                CustIDs = custIDs
            };

            return _repository.Fetch<int>(criteria);
        }
        #endregion

        #region InsertLatencyCustomer
        public IEnumerable<int> GetLatencyCustomerFromQueue(int batchSize)
        {
            var criteria = new GetLatencyCustomerFromQueueCriteria
            {
                BatchSize = batchSize
            };

            return _repository.Fetch<int>(criteria);
        }

        public void CompleteLatencyCustomerFromQueue(string custIds)
        {
            var criteria = new CompleteLatencyCustomerFromQueueCriteria
            {
                CustIds = custIds
            };

            _repository.Execute(criteria);
        }
        #endregion
    }
}