namespace Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Criteria;
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
        LastScannedCustModel GetLastInsertCust();

        InsertCustomerViewModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(int quantityPerBatch, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(string custIds);

        IEnumerable<InsertNewCustomerResultModel> InsertNewCTSCustomer(IEnumerable<CustomerMainDBModel> customerInfo);

        void UpdateAssociationCustomerForArchive(string custInfoJson);

        void UpdateAssociationCustomerForRollback(string custInfoJson);

        void UpdateCustInfoComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        void UpdateCustomerStatusComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        LastUpdateValueModel GetLastUpdatedTime();

        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        TranformCustModel GetUpdatedCustFromProductStatus(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);

        IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType);

        IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType);

        #region UserName2

        ScannedCustModel GetCustWithNullUserName2(int rowSize, Int64 lastCTSCustId, bool isCustSub);

        IEnumerable<CustNullUserName2Model> GetCustWithHavingUserName2(IEnumerable<CustNullUserName2Model> custs, bool isCustSub);

        void UpdateUsername2(IEnumerable<CustNullUserName2Model> custInfo);

        #endregion

        void UpdateCompleteAndRetranCustomer(string jsonData, long? lastCustID, bool isInsertMissingCust);

        IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime lastScannedDate);

        IEnumerable<long> CheckExistCTSArchiveCustomer(string custIDs);

        InactivateNewCustomer GetInactivateNewCustomers();
    }

    public class CTSCustomerDataAccess : ICTSCustomerDataAccess
    {
        private readonly IDynamicRepository repository;

        public CTSCustomerDataAccess()
        {
            repository = new DynamicRepository();
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

            return new LastScannedCustModel()
            {
                LastCustID = LastCustID,
                LastCustSubID = LastCustSubID
            };
        }

        public InsertCustomerViewModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustomersFromMainDbCriteria
            {
                LastCustId = lastScannedCustID,
                RowSize = quantityPerBatch,
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            using var result = db.QueryMultiple(criteria);
            var customers = result?.Read<CustomerMainDBModel>();
            var LastCustID = result?.Read<Int64?>();

            return new InsertCustomerViewModel()
            {
                CustomerInfos = customers?.ToList() ?? new List<CustomerMainDBModel>(),
                LastInsertCustID = LastCustID?.FirstOrDefault()
            };
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustSubFromMainDbCriteria
            {
                LastCustSubId = lastScannedCustID,
                RowSize = quantityPerBatch,
                QueryType = (int)QueryType.GetByRange,
                CustIdList = ""
            };

            return repository.Fetch<CustomerMainDBModel>(criteria) ?? Enumerable.Empty<CustomerMainDBModel>();
        }

        public IEnumerable<InsertNewCustomerResultModel> InsertNewCTSCustomer(IEnumerable<CustomerMainDBModel> customerInfo)
        {
            if (customerInfo != null && customerInfo.Any())
            {
                var criteria = new NewCTSCustomerInsertCriteria
                {
                    CustomerJson = JsonConvert.SerializeObject(customerInfo)
                };

                return repository.Fetch<InsertNewCustomerResultModel>(criteria) ?? Enumerable.Empty<InsertNewCustomerResultModel>();
            }

            return Enumerable.Empty<InsertNewCustomerResultModel>();
        }

        public void UpdateAssociationCustomerForArchive(string custInfoJson)
        {
            var criteria = new UpdateAssociationCustomerForArchiveCriteria
            {
                CustInfoJson = custInfoJson
            };

            repository.Execute(criteria);
        }

        public void UpdateAssociationCustomerForRollback(string custInfoJson)
        {
            var criteria = new UpdateAssociationCustomerForRollbackCriteria
            {
                CustInfoJson = custInfoJson
            };

            repository.Execute(criteria);
        }

        public void UpdateCompleteAndRetranCustomer(string jsonData, long? lastCustID, bool isInsertMissingCust)
        {
            var criteria = new UpdateCompleteAndRetranCustomerCriteria
            {
                CustomerInfo = jsonData,
                LastInsertCustID = lastCustID,
                IsInsertMissingCust = isInsertMissingCust
            };

            repository.Execute(criteria);
        }

        #endregion Insert New Customers

        #region Update Customers Info

        public TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromCustInfoHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            var (firstResultSet, secondResultSet) = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel
            {
                LastUpdatedTime = firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = firstResultSet.SingleOrDefault().LastCustId,
                CustIdList = secondResultSet.SingleOrDefault()
            };
        }

        public TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromCustomerHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            var (firstResultSet, secondResultSet) = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel
            {
                LastUpdatedTime = firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = firstResultSet.SingleOrDefault().LastCustId,
                CustIdList = secondResultSet.SingleOrDefault()
            };
        }

        public TranformCustModel GetUpdatedCustFromProductStatus(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromProductStatusHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            var (firstResultSet, secondResultSet) = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel
            {
                LastUpdatedTime = firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = firstResultSet.SingleOrDefault().LastCustId,
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

            repository.Execute(criteria);
        }

        public void UpdateCustInfoComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var criteria = new UpdateCustInfoCompleteCriteria
            {
                CustInfo = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            repository.Execute(criteria);
        }

        public void UpdateCustomerStatusComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var criteria = new UpdateCustomerStatusCompleteCriteria
            {
                CustProductStatus = customers,
                LastUpdateCustID = lastUpdateCustID,
                LastUpdateTime = lastUpdateTime
            };

            repository.Execute(criteria);
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType)
        {
            var criteria = new GetCustomerInfoFromMainDBCriteria
            {
                CustIds = custIds,
                SourceType = (int)sourceType
            };

            var custInfos = repository.Fetch<CustomerMainDBModel>(criteria);

            return custInfos;
        }

        public IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType)
        {
            var criteria = new GetCustomerInfoFromCTSCriteria
            {
                CustIds = custIds,
                SourceType = (int)sourceType
            };

            var custInfos = repository.Fetch<CustomerMainDBModel>(criteria);

            return custInfos;
        }

        #endregion Update Customers Info

        #region Update UserName2

        public ScannedCustModel GetCustWithNullUserName2(int rowSize, Int64 lastCTSCustId, bool isCustSub)
        {
            var criteria = new GetCustomersWithNullUsername2Criteria
            {
                Size = rowSize,
                PreCTSCustID = lastCTSCustId,
                IsCustSub = isCustSub
            };

            var data = repository.Fetch<CustNullUserName2Model>(criteria);

            return new ScannedCustModel
            {
                CustList = data ?? Enumerable.Empty< CustNullUserName2Model>(),
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

            return repository.Fetch<CustNullUserName2Model>(criteria);
        }

        public void UpdateUsername2(IEnumerable<CustNullUserName2Model> custInfo)
        {
            var criteria = new UpdateUsername2Criteria
            {
                CustomerJson = JsonConvert.SerializeObject(custInfo)
            };

            repository.Execute(criteria);
        }

        #endregion Update UserName2

        #region Insert Reactivated Customers

        public IEnumerable<ReactivatedCustModel> GetReactivatedCustomers(DateTime lastScannedDate)
        {
            var criteria = new ReactivatedCustomerGetCriteria
            {
                LastScannedDate = lastScannedDate
            };

            return repository.Fetch<ReactivatedCustModel>(criteria) ?? Enumerable.Empty<ReactivatedCustModel>();
        }

        public IEnumerable<long> CheckExistCTSArchiveCustomer(string custIDs)
        {
            var criteria = new CheckExistCTSArchiveCustomerCriteria
            {
                CustIDs = custIDs
            };

            return repository.Fetch<long>(criteria) ?? Enumerable.Empty<long>();
        }

        public IEnumerable<CustomerMainDBModel> GetCustSubFromMainDB(string custIds)
        {
            var criteria = new GetCustSubFromMainDbCriteria
            {
                CustIdList = custIds,
                QueryType = (int)QueryType.GetByCustId
            };

            return repository.Fetch<CustomerMainDBModel>(criteria) ?? Enumerable.Empty<CustomerMainDBModel>();
        }

        #endregion Insert Reactivated Customers

        #region Inactive New Customers

        public InactivateNewCustomer GetInactivateNewCustomers()
        {
            var data = new InactivateNewCustomer();
            var criteria = new GetInactivateNewCustomerCriteria();

            data.CustInfos = repository.Fetch<RescanCategoryDailyModel>(criteria) ?? Enumerable.Empty<RescanCategoryDailyModel>();
            data.LastCustID = criteria.LastCustID;
            data.LastCategoryID = criteria.LastCategoryID;

            return data;
        }

        #endregion Inactive New Customers
    }
}