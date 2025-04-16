namespace Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingLatencyCustomer
{
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.MissingCustomer.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CTSCustomer.Models;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface IMissingCustomerService
    {
        void InsertMissingCustomerByLatencyToQueue(IEnumerable<int> custIds);

        InsertCustomerModel CheckCustomerInfoFromMainDB(IEnumerable<int> custIds);

        #region ScanLatencyCustomer
        int GetLastScannedCustIDByScannedSecondTime(int fromCustID, int toCustID, int scannedTime);

        ScanLatencyCustomerEntity ScanLatencyCustomer(int fromCustID, int toCustID, int batchSize);

        IEnumerable<int> CheckMissingCustomer(IEnumerable<int> custIDs);
        #endregion

        #region InsertLatencyCustomer
        IEnumerable<int> GetLatencyCustomerFromQueue(int batchSize);

        void CompleteLatencyCustomerFromQueue(IEnumerable<int> custIds);
        #endregion
    }

    public class MissingCustomerService : IMissingCustomerService
    {
        private readonly IMissingCustomerDataAccess _dataAccess;

        public MissingCustomerService(IMissingCustomerDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void InsertMissingCustomerByLatencyToQueue(IEnumerable<int> custIds)
        {
            if (custIds.IsNotNullOrEmpty())
            {
                var custIdToString = string.Join(",", custIds);
                _dataAccess.InsertMissingCustomerByLatencyToQueue(custIdToString);
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
        public int GetLastScannedCustIDByScannedSecondTime(int fromCustID, int toCustID, int scannedTime)
        {
            return _dataAccess.GetLastScannedCustIDByScannedSecondTime(fromCustID, toCustID, scannedTime);
        }

        public ScanLatencyCustomerEntity ScanLatencyCustomer(int fromCustID, int toCustID, int batchSize)
        {
            return _dataAccess.ScanLatencyCustomer(fromCustID, toCustID, batchSize);
        }

        public IEnumerable<int> CheckMissingCustomer(IEnumerable<int> custIDs)
        {
            if (custIDs.IsNotNullOrEmpty())
            {
                var custIDToString = string.Join(",", custIDs);
                return _dataAccess.CheckMissingCustomer(custIDToString);
            }

            return Enumerable.Empty<int>();
        }
        #endregion

        #region InsertLatencyCustomer
        public IEnumerable<int> GetLatencyCustomerFromQueue(int batchSize)
        {
            return _dataAccess.GetLatencyCustomerFromQueue(batchSize);
        }

        public void CompleteLatencyCustomerFromQueue(IEnumerable<int> custIds)
        {
            if (custIds.IsNotNullOrEmpty())
            {
                var custIdToString = string.Join(",", custIds);
                _dataAccess.CompleteLatencyCustomerFromQueue(custIdToString);
            }
        }
        #endregion
    }
}
