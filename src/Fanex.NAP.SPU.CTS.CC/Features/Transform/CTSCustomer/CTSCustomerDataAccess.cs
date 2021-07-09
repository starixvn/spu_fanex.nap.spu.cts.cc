namespace Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.Features.Transform.CTSCustomer.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICTSCustomerDataAccess
    {
        LastScannedCustModel GetLastInsertCust();

        UpdateCustomerModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID);

        List<CustomerMainDBModel> GetCustSubFromMainDB(int quantityPerBatch, Int64 lastScannedCustID);

        IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(string customerJson);

        void UpdateCustInfoComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID);

        LastUpdateValueModel GetLastUpdatedTime();
        TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);
        TranformCustModel GetUpdatedCustFromCustomerHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount);
        IEnumerable<CustomerMainDBModel> GetCustomerFromMainDBForUpdate(string custIds, UpdateCustomer sourceType);
        IEnumerable<CustomerMainDBModel> GetCustomerFromCTSForUpdate(string custIds, UpdateCustomer sourceType);
        IEnumerable<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId);
        IEnumerable<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs);
        int UpdateUsername2(string customerJson);
    }

    public class CTSCustomerDataAccess : ICTSCustomerDataAccess
    {
        readonly IDynamicRepository repository;

        public CTSCustomerDataAccess()
        {
            repository = new DynamicRepository();
        }


        public LastUpdateValueModel GetLastUpdatedTime()
        {
            var criteria = new GetLastUpdatedTimeCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var LastUpdateTimeCustInfo = result.Read<DateTime>().FirstOrDefault();
                    var LastUpdateCustIDCustInfo = result.Read<Int64>().FirstOrDefault();
                    var LastUpdateTimeCustomer = result.Read<DateTime>().FirstOrDefault();
                    var LastUpdateCustIDCustomer = result.Read<Int64>().FirstOrDefault();

                    return new LastUpdateValueModel()
                    {
                        LastUpdateCustIDCustInfo = LastUpdateCustIDCustInfo,
                        LastUpdateTimeCustInfo = LastUpdateTimeCustInfo,
                        LastUpdateCustIDCustomer = LastUpdateCustIDCustomer,
                        LastUpdateTimeCustomer = LastUpdateTimeCustomer
                    };
                }
            }

        }

        public TranformCustModel GetUpdatedCustFromCustInfoHistory(DateTime lastUpdatedTime, Int64 lastUpdatedCustId, int numOfAccount)
        {
            var criteria = new GetUpdatedCustFromCustInfoHistoryCriteria
            {
                LastUpdateTime = lastUpdatedTime,
                LastCustId = lastUpdatedCustId,
                RowSize = numOfAccount,
            };

            var repository = new DynamicRepository();

            var output = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel()
            {
                LastUpdatedTime = output.firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = output.firstResultSet.SingleOrDefault().LastCustId,
                CustIdList = output.secondResultSet.SingleOrDefault()
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

            var repository = new DynamicRepository();

            var output = repository.FetchMultiple<TranformCustModel, string>(criteria);

            return new TranformCustModel()
            {
                LastUpdatedTime = output.firstResultSet.SingleOrDefault().LastUpdatedTime,
                LastCustId = output.firstResultSet.SingleOrDefault().LastCustId,
                CustIdList = output.secondResultSet.SingleOrDefault()
            };
        }

        public LastScannedCustModel GetLastInsertCust()
        {
            var criteria = new GetLastInsertCustCriteria();

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var LastCustID = result.Read<Int64>().FirstOrDefault();
                    var LastCustSubID = result.Read<Int64>().FirstOrDefault();

                    return new LastScannedCustModel()
                    {
                        LastCustID = LastCustID,
                        LastCustSubID = LastCustSubID
                    };
                }
            }
        }

        public UpdateCustomerModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustomersFromMainDbCriteria
            {
                LastCustId = lastScannedCustID,
                RowSize = quantityPerBatch,
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var result = db.QueryMultiple(criteria))
                {
                    var customers = result?.Read<CustomerMainDBModel>();
                    var LastCustID = result?.Read<Int64?>();

                    return new UpdateCustomerModel()
                    {
                        CustomerInfos = customers.ToList(),
                        LastInsertCustID = LastCustID?.FirstOrDefault()
                    };
                }
            }
        }

        public List<CustomerMainDBModel> GetCustSubFromMainDB(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustSubFromMainDbCriteria
            {
                LastCustSubId = lastScannedCustID,
                RowSize = quantityPerBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<CustomerMainDBModel>(criteria).ToList();
            }
        }

        public IEnumerable<InsertCustomerModel> TransformCustomerFromMainDB(string customerJson)
        {
            var custInfos = new List<InsertCustomerModel>();

            var criteria = new TransformCustomerFromMainDbCriteria
            {
                CustomerJson = customerJson
            };

            return repository.Fetch<InsertCustomerModel>(criteria);
        }

        public void UpdateCustomerComplete(string customers, DateTime? lastUpdateTime, Int64? lastUpdateCustID)
        {
            var repository = new DynamicRepository();

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
            var repository = new DynamicRepository();

            var criteria = new UpdateCustInfoCompleteCriteria
            {
                CustInfo = customers,
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

        public IEnumerable<GetCustIdModel> GetCustWithNullUserName2(int rowSize, Int64 preCTSCustId)
        {
            var criteria = new GetCustomersWithNullUsername2Criteria
            {
                Size = rowSize,
                PreCTSCustID = preCTSCustId
            };

            return repository.Fetch<GetCustIdModel>(criteria);
        }

        public IEnumerable<GetCustIdModel> GetCustWithHavingUserName2(List<GetCustIdModel> custs)
        {
            var criteria = new GetCustomersWithHavingUsername2Criteria
            {
                CustomerXML = custs.ToXmlString()
            };

            return repository.Fetch<GetCustIdModel>(criteria);
        }

        public int UpdateUsername2(string customerJson)
        {
            var criteria = new UpdateUsername2Criteria
            {
                CustomerJson = customerJson
            };

            return repository.Execute(criteria);
        }
    }
}
