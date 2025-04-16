namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
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
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerDataAccess
    {
        IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByCustIDs(string custIds);

        void UpdateCustInfoComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        void UpdateCustomerStatusComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        LastUpdateValueModel GetLastUpdatedTime();

        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime);

        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime);

        TranformCustModel GetUpdatedCustFromProductStatus(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount, int waitTime);

        IEnumerable<CustomerMainDBEntity> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType);

        IEnumerable<CustomerMainDBEntity> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType);

        #region Insert Missing Customers

        public IEnumerable<MissingCustomerModel> GetMissingCustomers();

        void CompleteMissingCustomer(string resolvedIDs);

        #endregion

        #region UserName2

        ScannedCustModel GetCustWithNullUserName2(int rowSize, UInt64 lastCTSCustId, bool isCustSub);

        IEnumerable<CustNullUserName2Model> GetCustWithHavingUserName2(IEnumerable<CustNullUserName2Model> custs, bool isCustSub);

        void UpdateUsername2(IEnumerable<CustNullUserName2Model> custInfo);

        #endregion

        IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime lastScannedDate, DateTime? nextScannedDate);

        IEnumerable<long> CheckExistCTSArchiveCustomer(string custJson);

        InactivateNewCustomer GetInactivateNewCustomers();

        void InsertReactivedCustomerLog(DateTime? lastScanTime, DateTime? requestTime, DateTime? maxLogTime, DateTime? custInfoTime);

        void UpdateAssociationCustomerForRollback(string custInfoJson);
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
        #endregion Insert New Customers

        #region Insert Missing Customer

        public IEnumerable<MissingCustomerModel> GetMissingCustomers()
        {
            var criteria = new GetMissingCustomerCriteria();

            return _repository.Fetch<MissingCustomerModel>(criteria);
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

        public void UpdateAssociationCustomerForRollback(string custInfoJson)
        {
            var criteria = new UpdateAssociationCustomerForRollbackCriteria
            {
                CustInfoJson = custInfoJson
            };

            _repository.Execute(criteria);
        }
    }
}