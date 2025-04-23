namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Models;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IMissingCustomerService
    {
        void InsertMissingCustomerByLatencyToQueue(IEnumerable<ScanLatencyCustomerEntity> scanLatencyCustomers, JobMonitor_IssueType jobMonitor_IssueType);

        void InsertMissingCustIdByLatencyToQueue(IEnumerable<int> custIds, JobMonitor_IssueType jobMonitor_IssueType);

        InsertCustomerModel CheckCustomerInfoFromMainDB(IEnumerable<int> custIds);

        #region ScanLatencyCustomer
        int? GetLastScannedCustIDByScannedSecondTime(int fromCustID, int toCustID, int scannedTime);

        ScanLatencyCustomerModel ScanLatencyCustomer(int fromCustID, int toCustID, int batchSize);

        IEnumerable<int> CheckMissingCustomer(IEnumerable<int> custIDs);
        #endregion

        #region InsertLatencyCustomer
        IEnumerable<GetLatencyCustomerFromQueueEntity> GetLatencyCustomerFromQueue(int batchSize);

        void CompleteLatencyCustomerFromQueue(IEnumerable<ComplateLatencyCustomerModel> customers);
        #endregion
    }

    public class MissingCustomerService : IMissingCustomerService
    {
        private readonly IMissingCustomerDataAccess _dataAccess;

        public MissingCustomerService(IMissingCustomerDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void InsertMissingCustomerByLatencyToQueue(IEnumerable<ScanLatencyCustomerEntity> scanLatencyCustomers, JobMonitor_IssueType jobMonitor_IssueType)
        {
            if (scanLatencyCustomers.IsNotNullOrEmpty())
            {
                var custJson = JsonConvert.SerializeObject(scanLatencyCustomers);
                _dataAccess.InsertMissingCustomerByLatencyToQueue(custJson, jobMonitor_IssueType);
            }
        }

        public void InsertMissingCustIdByLatencyToQueue(IEnumerable<int> custIds, JobMonitor_IssueType jobMonitor_IssueType)
        {
            if (custIds.IsNotNullOrEmpty())
            {
                var customers = custIds.Select(id => new ScanLatencyCustomerEntity { CustID = id });
                InsertMissingCustomerByLatencyToQueue(customers, jobMonitor_IssueType);
            }
        }

        public InsertCustomerModel CheckCustomerInfoFromMainDB(IEnumerable<int> custIds)
        {
            if (custIds.IsNotNullOrEmpty())
            {
                var custIdToString = string.Join(",", custIds);

                return _dataAccess.CheckCustomerInfoFromMainDB(custIdToString);
            }

            return new InsertCustomerModel();
        }

        #region ScanLatencyCustomer
        public int? GetLastScannedCustIDByScannedSecondTime(int fromCustID, int toCustID, int scannedTime)
        {
            return _dataAccess.GetLastScannedCustIDByScannedSecondTime(fromCustID, toCustID, scannedTime);
        }

        public ScanLatencyCustomerModel ScanLatencyCustomer(int fromCustID, int toCustID, int batchSize)
        {
            return _dataAccess.ScanLatencyCustomer(fromCustID, toCustID, batchSize);
        }

        public IEnumerable<int> CheckMissingCustomer(IEnumerable<int> custIDs)
        {
            if (custIDs.IsNotNullOrEmpty())
            {
                var jsonData = JsonConvert.SerializeObject(custIDs.Select(x => new CheckMissingCustomerModel { CustID = x }));
                return _dataAccess.CheckMissingCustomer(jsonData);
            }

            return Enumerable.Empty<int>();
        }
        #endregion

        #region InsertLatencyCustomer
        public IEnumerable<GetLatencyCustomerFromQueueEntity> GetLatencyCustomerFromQueue(int batchSize)
        {
            return _dataAccess.GetLatencyCustomerFromQueue(batchSize);
        }

        public void CompleteLatencyCustomerFromQueue(IEnumerable<ComplateLatencyCustomerModel> customers)
        {
            if (customers.IsNotNullOrEmpty())
            {
                var customerJson = JsonConvert.SerializeObject(customers);
                _dataAccess.CompleteLatencyCustomerFromQueue(customerJson);
            }
        }
        #endregion
    }
}
