namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationBySportServices
    {
        IDailyNormalClassificationBySportServices Start();

        IDailyNormalClassificationBySportServices GetCustClassificationForDailyScanning();

        IDailyNormalClassificationBySportServices ScanningDailyNormalClassification();

        bool UpdateLastScan();
    }

    public class DailyNormalClassificationBySportServices : IDailyNormalClassificationBySportServices
    {
        private readonly IDailyNormalClassificationBySportDataAccess dailyDataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private DailyNormalAccountsClassificationBySportEntity scanningCustomers;

        public DailyNormalClassificationBySportServices(IDailyNormalClassificationBySportDataAccess dailyDataAccess
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

        public IDailyNormalClassificationBySportServices ScanningDailyNormalClassification()
        {
            if (scanningCustomers != null && scanningCustomers.NormalAccounts != null && scanningCustomers.NormalAccounts.Any())
            {                
                this.dailyDataAccess.ScanDailyNormalAccountsClassificationBySport(scanningCustomers.NormalAccounts.ToXmlString());
            }

            return this;
        }

        public bool UpdateLastScan()
        {
            if (this.scanningCustomers != null)
            {                
                systemParameterService.UpdateTimingExecution(scanningCustomers.LastCategoryID.ToString(), SystemParameters.DailyNormalClassificationBySportLastCategoryID);               
                systemParameterService.UpdateTimingExecution(scanningCustomers.LastCustID.ToString(), SystemParameters.DailyNormalClassificationBySportLastCustID);

                return (this.scanningCustomers.NormalAccounts != null && this.scanningCustomers.NormalAccounts.Any());
            }

            return false;
        }
    }
}
