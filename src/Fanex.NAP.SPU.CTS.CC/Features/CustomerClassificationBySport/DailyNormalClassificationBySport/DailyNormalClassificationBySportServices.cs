namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationBySportServices
    {
        IDailyNormalClassificationBySportServices Start();

        IDailyNormalClassificationBySportServices GetCustClassificationForDailyScanning();

        IDailyNormalClassificationBySportServices ScanningDailyNormalClassification(FunctionId funcID);

        bool UpdateLastScan();
    }

    public class DailyNormalClassificationBySportServices : IDailyNormalClassificationBySportServices
    {
        private readonly DailyNormalClassificationBySportDataAccess dailyDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private DailyNormalAccountsClassificationBySportEntity scanningCustomers;

        public DailyNormalClassificationBySportServices(DailyNormalClassificationBySportDataAccess dailyDataAccess
            , ISystemParameterServices systemParameterService)
        {
            this.dailyDataAccess = dailyDataAccess;
            this.systemParameterService = systemParameterService;
        }

        public IDailyNormalClassificationBySportServices Start()
        {
            this.scanningCustomers = new DailyNormalAccountsClassificationBySportEntity();

            return this;
        }

        public IDailyNormalClassificationBySportServices GetCustClassificationForDailyScanning()
        {
            this.scanningCustomers = this.dailyDataAccess.GetDailyNormalAccountsClassificationBySport();

            return this;
        }

        public IDailyNormalClassificationBySportServices ScanningDailyNormalClassification(FunctionId funcID)
        {
            if (scanningCustomers != null && scanningCustomers.CustID != null && scanningCustomers.CustID.Any())
            {
                var custIds = string.Join(",", scanningCustomers.CustID);

                this.dailyDataAccess.ScanDailyNormalAccountsClassificationBySport(custIds, funcID);
            }

            return this;
        }

        public bool UpdateLastScan()
        {
            if (this.scanningCustomers != null)
            {                
                systemParameterService.UpdateTimingExecution(scanningCustomers.LastCategoryID.ToString(), SystemParameters.DailyNormalClassificationBySportLastCategoryID);
                systemParameterService.UpdateTimingExecution(scanningCustomers.LastSportID.ToString(), SystemParameters.DailyNormalClassificationBySportLastSportID);
                systemParameterService.UpdateTimingExecution(scanningCustomers.LastCustID.ToString(), SystemParameters.DailyNormalClassificationBySportLastCustID);

                return (this.scanningCustomers.CustID != null && this.scanningCustomers.CustID.Any());
            }

            return false;
        }
    }
}
