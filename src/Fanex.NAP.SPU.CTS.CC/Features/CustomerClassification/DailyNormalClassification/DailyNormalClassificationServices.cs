namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationServices
    {
        IDailyNormalClassificationServices Start();

        IDailyNormalClassificationServices GetCustClassificationForDailyScanning();

        IDailyNormalClassificationServices ScanningDailyNormalClassification(FunctionId funcId, bool isCheckLast14Days);

        void ScanningDailyNormalClassification(IEnumerable<long> customers, FunctionId funcId, bool isCheckLast14Days);

        bool UpdateLastScan();
    }

    public class DailyNormalClassificationServices : IDailyNormalClassificationServices
    {
        private readonly IDailyNormalClassificationDataAccess dailyClassificationDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private NormalClassificationDailyEntity scanningCustomers;

        public DailyNormalClassificationServices(
            IDailyNormalClassificationDataAccess dailyClassificationDataAccess,
            ISystemParameterServices systemParameterService)
        {
            this.dailyClassificationDataAccess = dailyClassificationDataAccess;
            this.systemParameterService = systemParameterService;
        }

        public IDailyNormalClassificationServices Start()
        {
            scanningCustomers = new NormalClassificationDailyEntity();

            return this;
        }

        public IDailyNormalClassificationServices GetCustClassificationForDailyScanning()
        {
            scanningCustomers = dailyClassificationDataAccess.GetCustomerForDailyNormalScanning();

            return this;
        }

        public IDailyNormalClassificationServices ScanningDailyNormalClassification(FunctionId funcId, bool isCheckLast14Days)
        {
            if (scanningCustomers != null && scanningCustomers.CustID != null && scanningCustomers.CustID.Any())
            {
                var custIds = string.Join(",", scanningCustomers.CustID);

                dailyClassificationDataAccess.ScanningDailyNormalClassification(custIds, funcId, isCheckLast14Days);
            }

            return this;
        }

        public void ScanningDailyNormalClassification(IEnumerable<long> customers, FunctionId funcId, bool isCheckLast14Days)
        {
            if (customers != null && customers.Any())
            {
                var custIdToString = string.Join(",", customers);

                dailyClassificationDataAccess.ScanningDailyNormalClassification(custIdToString, funcId, isCheckLast14Days);
            }
        }

        public bool UpdateLastScan()
        {
            if (scanningCustomers != null)
            {
                systemParameterService.UpdateTimingExecution(scanningCustomers.op_LastCustID.ToString(), SystemParameters.DailyNormalClassificationLastCustID);
                systemParameterService.UpdateTimingExecution(scanningCustomers.op_LastCategoryID.ToString(), SystemParameters.DailyNormalClassificationLastCategoryID);

                return (scanningCustomers.CustID != null && scanningCustomers.CustID.Any());
            }

            return false;
        }
    }
}