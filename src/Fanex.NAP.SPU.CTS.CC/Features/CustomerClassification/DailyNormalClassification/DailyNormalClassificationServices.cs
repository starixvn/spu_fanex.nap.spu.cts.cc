namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationServices
    {
        IDailyNormalClassificationServices Start();

        IDailyNormalClassificationServices GetCustomerForDailyNormalScanning();

        IDailyNormalClassificationServices ClassifyDailyNormal();

        bool UpdateLastScan();
    }

    public class DailyNormalClassificationServices : IDailyNormalClassificationServices
    {
        private readonly IDailyNormalClassificationDataAccess dailyClassificationDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private DailyNormalClassificationEntity scanningCustomers;

        public DailyNormalClassificationServices(
            IDailyNormalClassificationDataAccess dailyClassificationDataAccess,
            ISystemParameterServices systemParameterService)
        {
            this.dailyClassificationDataAccess = dailyClassificationDataAccess;
            this.systemParameterService = systemParameterService;
        }

        public IDailyNormalClassificationServices Start()
        {
            scanningCustomers = new DailyNormalClassificationEntity();

            return this;
        }

        public IDailyNormalClassificationServices GetCustomerForDailyNormalScanning()
        {
            scanningCustomers = dailyClassificationDataAccess.GetCustomerForDailyNormalScanning();

            return this;
        }

        public IDailyNormalClassificationServices ClassifyDailyNormal()
        {
            if (scanningCustomers != null && scanningCustomers.CustID != null && scanningCustomers.CustID.Any())
            {
                var custIds = string.Join(",", scanningCustomers.CustID.Distinct());

                dailyClassificationDataAccess.ClassifyDailyNormal(custIds);
            }

            return this;
        }

        public bool UpdateLastScan()
        {
            if (scanningCustomers != null)
            {
                systemParameterService.UpdateTimingExecution(scanningCustomers.op_LastCustID.ToString(), CTS_SystemParameters.DailyNormalClassificationLastCustID);
                systemParameterService.UpdateTimingExecution(scanningCustomers.op_LastCategoryID.ToString(), CTS_SystemParameters.DailyNormalClassificationLastCategoryID);

                return (scanningCustomers.CustID != null && scanningCustomers.CustID.Any());
            }

            return false;
        }
    }
}