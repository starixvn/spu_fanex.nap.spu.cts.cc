namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification
{  
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationServices
    {
        IDailyNormalClassificationServices Start();

        IDailyNormalClassificationServices GetCustClassificationForDailyScanning();

        IDailyNormalClassificationServices ScanningDailyNormalClassification(FunctionId funcId);

        void ScanningDailyNormalClassification(IEnumerable<long> customers, FunctionId funcId);
        
        bool UpdateLastScan();
    
    }

    public class DailyNormalClassificationServices : IDailyNormalClassificationServices
    {
        private readonly IDailyNormalClassificationDataAccess dailyClassificationDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private NormalClassificationDailyEntity scanningCustomers;
        private readonly ISentryClient sentryClient;

        public DailyNormalClassificationServices(
            IDailyNormalClassificationDataAccess dailyClassificationDataAccess,
            ISystemParameterServices systemParameterService,
            ISentryClient sentryClient)
        {
            this.dailyClassificationDataAccess = dailyClassificationDataAccess;
            this.systemParameterService = systemParameterService;
            this.sentryClient = sentryClient;
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

        public IDailyNormalClassificationServices ScanningDailyNormalClassification(FunctionId funcId)
        {
            if (scanningCustomers != null && scanningCustomers.CustID != null && scanningCustomers.CustID.Any())
            {
                var logCustIDs = new List<long>() { 1296, 1331, 1335, 1447036, 1446906, 1447622 };

                var logCusts = scanningCustomers.CustID.Where(x => logCustIDs.Contains(x));

                Utilities.LogSentryInfo("DailyNormalClassification", "ScanningDailyNormalClassification scanningCustomers", logCusts, true, sentryClient);

                var custIds = string.Join(",", scanningCustomers.CustID);
                
                var custNormalInfos = dailyClassificationDataAccess.GetExistingCustClassificationNormal(custIds);

                var logCustNormalInfos = custNormalInfos.Where(x => logCustIDs.Contains(x.CustID));

                Utilities.LogSentryInfo("DailyNormalClassification", "GetExistingCustClassificationNormal custNormalInfos", custNormalInfos, true, sentryClient);

                if (custNormalInfos.Any())
                {
                    dailyClassificationDataAccess.ScanningDailyNormalClassification(custNormalInfos.Distinct().ToXmlString(), funcId);
                }
            }

            return this;
        }

        public void ScanningDailyNormalClassification(IEnumerable<long> customers, FunctionId funcId)
        {
            if (customers != null && customers.Any())
            {
                var custIdToString = string.Join(",", customers);

                var custNormalInfos = dailyClassificationDataAccess.GetExistingCustClassificationNormal(custIdToString);

                if (custNormalInfos.Any())
                {
                    dailyClassificationDataAccess.ScanningDailyNormalClassification(custNormalInfos.Distinct().ToXmlString(), funcId);
                }                
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