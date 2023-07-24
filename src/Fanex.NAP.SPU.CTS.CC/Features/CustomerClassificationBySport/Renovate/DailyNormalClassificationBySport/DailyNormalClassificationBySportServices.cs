namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.DailyNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationBySportServices
    {
        IDailyNormalClassificationBySportServices Start();

        IDailyNormalClassificationBySportServices GetCustClassificationForDailyScanning();

        IDailyNormalClassificationBySportServices PreprocessDailyNormalClassification();

        bool UpdateLastScan();
    }

    public class DailyNormalClassificationBySportServices : IDailyNormalClassificationBySportServices
    {
        private readonly IDailyNormalClassificationBySportDataAccess dataAccess;
        private readonly ISystemParameterServices systemParameterService;
        private NormalAccountBySportModel customers;
        private readonly ISentryClient sentryClient;

        public DailyNormalClassificationBySportServices(IDailyNormalClassificationBySportDataAccess dataAccess
            , ISystemParameterServices systemParameterService
            , ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.systemParameterService = systemParameterService;
            this.sentryClient = sentryClient;
        }

        public IDailyNormalClassificationBySportServices Start()
        {
            this.customers = new NormalAccountBySportModel();

            return this;
        }

        public IDailyNormalClassificationBySportServices GetCustClassificationForDailyScanning()
        {
            this.customers = this.dataAccess.GetDailyNormalAccountsClassificationBySport();

            Utilities.LogSentryInfo("BySportDaily", "Get data done", customers, true, sentryClient);

            return this;
        }

        public IDailyNormalClassificationBySportServices PreprocessDailyNormalClassification()
        {
            if (customers !=null && customers.NormalAccounts != null && customers.NormalAccounts.Any())
            {
                this.dataAccess.PreprocessDailyNormalAccountsClassificationBySport(customers.NormalAccounts.ToXmlString());                
            }

            return this;
        }
       
        public bool UpdateLastScan()
        {
            if (this.customers != null)
            {                
                systemParameterService.UpdateTimingExecution(customers.LastCategoryID.ToString(), SystemParameters.DailyNormalClassificationBySportLastCategoryID);
                systemParameterService.UpdateTimingExecution(customers.LastCustID.ToString(), SystemParameters.DailyNormalClassificationBySportLastCustID);
                               
                return (this.customers.NormalAccounts != null && this.customers.NormalAccounts.Any());
            }

            return false;
        }
    }
}
