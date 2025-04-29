namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IInsertNewCustomerDataAccess
    {
        LastScannedCustModel GetLastInsertCust();

        InsertCustomerModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID);

        IEnumerable<CustomerMainDBEntity> GetCustSubFromMainDBByLastCustSub(int quantityPerBatch, Int64 lastScannedCustID);

        IEnumerable<InsertNewCustomerResultModel> InsertNewCTSCustomer(IEnumerable<CustomerMainDBEntity> customerInfo, bool isNewCustomer);

        void UpdateAssociationCustomerForArchive(string custInfoJson);

        void UpdateCompleteAndRetranCustomer(string jsonData, long? lastCustID, bool isInsertMissingCust);
    }

    public class InsertNewCustomerDataAccess : IInsertNewCustomerDataAccess
    {
        private readonly IDynamicRepository _repository;

        public InsertNewCustomerDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public LastScannedCustModel GetLastInsertCust()
        {
            var criteria = new GetLastInsertCustCriteria();

            (var lastCustIDs, var lastCustSubIDs) = _repository.FetchMultiple<string, string>(criteria);

            return new LastScannedCustModel
            {
                LastCustID = long.Parse(lastCustIDs.FirstOrDefault()),
                LastCustSubID = long.Parse(lastCustSubIDs.FirstOrDefault())
            };
        }

        public InsertCustomerModel GetCustomerFromMainDBForInsertNew(int quantityPerBatch, Int64 lastScannedCustID)
        {
            var criteria = new GetCustomersFromMainDbCriteria
            {
                LastCustId = lastScannedCustID,
                RowSize = quantityPerBatch,
            };

            (var customers, var missingCustIds, var lastCustID) = _repository.FetchMultiple<CustomerMainDBEntity, int, int?>(criteria);

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
    }
}
