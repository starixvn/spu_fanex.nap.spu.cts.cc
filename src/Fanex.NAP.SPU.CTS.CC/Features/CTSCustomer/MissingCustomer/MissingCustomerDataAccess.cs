namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.InsertNewCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.Transform.CTSCustomer.Criteria;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMissingCustomerDataAccess
    {
        void InsertMissingCustomerByLatencyToQueue(string custIds);

        InsertCustomerModel CheckCustomerInfoFromMainDB(string custIds);

        #region ScanLatencyCustomer
        int GetLastScannedCustIDByScannedSecondTime(int fromCustID, int toCustID, int scannedTime);

        ScanLatencyCustomerEntity ScanLatencyCustomer(int fromCustID, int toCustID, int batchSize);

        IEnumerable<int> CheckMissingCustomer(string custIDs);
        #endregion

        #region InsertLatencyCustomer
        IEnumerable<int> GetLatencyCustomerFromQueue(int batchSize);

        void CompleteLatencyCustomerFromQueue(string custIds);
        #endregion
    }

    public class MissingCustomerDataAccess : IMissingCustomerDataAccess
    {
        private readonly IDynamicRepository _repository;

        public MissingCustomerDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public void InsertMissingCustomerByLatencyToQueue(string custIds)
        {
            var criteria = new InsertMissingCustomerLatencyToQueueCriteria
            {
                CustIDs = custIds
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

        #region ScanLatencyCustomer
        public int GetLastScannedCustIDByScannedSecondTime(int fromCustID, int toCustID, int scannedTime)
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
