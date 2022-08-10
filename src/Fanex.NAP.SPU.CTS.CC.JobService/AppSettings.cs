namespace Fanex.NAP.SPU.CTS.CC.JobService
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.ComponentModel;

    public interface IAppSettings
    {
        int Environment { get; }

        string EvironmentName { get; }

        string SPName { get; }

        string Soon88FeedDataAPI { get; }

        string AFCInsertAccountURL { get; }

        string AFCDeleteAccountURL { get; }

        bool IsSyncAFCData { get; }

        bool ProblemAccountIsWriteLog { get; }

        bool NormalAccountIsWriteLog { get; }

        bool RobotAccountIsWriteLog { get; }

        bool RemoveAccountEvidenceIsWriteLog { get; }

        bool CustomerEvidenceIsSendMailToSPU { get; }

        int CustomerEvidenceMailingToSPU { get; }

        bool TransformCTSCustomerIsWriteLog { get; }

        int TransformCTSCustomerBatchSize { get; }

        bool MonitorInsertNewCustIsAutoInsertMissingCust { get; }

        bool InsertReactivatedCustomersIsWriteLog { get; }

        int InsertReactivatedCustomersBatchSize { get; }

        string SentryDsn { get; }

        string InsertVIPURL { get; }

        string InsertBAURL { get; }

        bool UpdateIsWriteLogSetting(string serviceName, bool isWriteLog);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string environment;
        private readonly string evironmentName;
        private readonly string spName;
        private readonly string soon88FeedDataAPI;
        private readonly string afcInsertAccountURL;
        private readonly string afcDeleteAccountURL;
        private readonly bool isSyncAFCData;
        private readonly string sentryDsn;
        private readonly string insertVIPURL;
        private readonly string insertBAURL;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            environment = GetValue<string>(nameof(Environment));
            evironmentName = GetValue<string>(nameof(Environment));
            spName = GetValue<string>(nameof(SPName));
            soon88FeedDataAPI = GetValue<string>(nameof(Soon88FeedDataAPI));
            afcInsertAccountURL = GetValue<string>("AntiFraudCenter:AFCInsertAccountURL");
            afcDeleteAccountURL = GetValue<string>("AntiFraudCenter:AFCDeleteAccountURL");
            isSyncAFCData = GetValue<bool>("AntiFraudCenter:IsSyncAFCData");
            sentryDsn = GetValue<string>("Sentry:Dsn");
            insertVIPURL = GetValue<string>("InsertVIP_BA_API:InsertVIPURL");
            insertBAURL = GetValue<string>("InsertVIP_BA_API:InsertBAURL");
        }

        #region Variables

        public int Environment
        {
            get
            {
                switch (environment)
                {
                    case "LOCAL":
                        return 1;

                    case "UAT":
                        return 2;

                    default:
                        return 3;
                }
            }
        }

        public string EvironmentName => evironmentName;

        public string SPName => spName;

        public string Soon88FeedDataAPI => soon88FeedDataAPI;

        public string AFCInsertAccountURL => afcInsertAccountURL;

        public string AFCDeleteAccountURL => afcDeleteAccountURL;

        public bool IsSyncAFCData => isSyncAFCData;

        public string SentryDsn => sentryDsn;

        public string InsertVIPURL => insertVIPURL;

        public string InsertBAURL => insertBAURL;

        public bool ProblemAccountIsWriteLog => GetValue<bool>("ProblemAccountInsert:IsWriteLog");

        public bool NormalAccountIsWriteLog => GetValue<bool>("NormalAccountInsert:IsWriteLog");

        public bool RobotAccountIsWriteLog => GetValue<bool>("RobotAccountInsert:IsWriteLog");

        public bool RemoveAccountEvidenceIsWriteLog => GetValue<bool>("RemoveAccountEvidence:IsWriteLog");

        public bool CustomerEvidenceIsSendMailToSPU => GetValue<bool>("CustomerEvidenceInsert:IsSendMailToSPU");

        public int CustomerEvidenceMailingToSPU => GetValue<int>("CustomerEvidenceInsert:MailingIdToSPU");

        public bool TransformCTSCustomerIsWriteLog => GetValue<bool>("TransformCTSCustomer:IsWriteLog");

        public int TransformCTSCustomerBatchSize => GetValue<int>("TransformCTSCustomer:BatchSize");

        public bool MonitorInsertNewCustIsAutoInsertMissingCust => GetValue<bool>("MonitorInsertNewCustomer:IsAutoInsertMissingCust");

        public bool InsertReactivatedCustomersIsWriteLog => GetValue<bool>("InsertReactivatedCustomers:IsWriteLog");

        public int InsertReactivatedCustomersBatchSize => GetValue<int>("InsertReactivatedCustomers:BatchSize");

        #endregion Variables

        #region Methods

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var section = configuration.GetSection(key);

            return section.Get<T>();
        }

        public void SetValue<T>(string key, T value)
        {
            try
            {
                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                configuration[key] = typeConverter.ConvertToString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public bool UpdateIsWriteLogSetting(string serviceName, bool isWriteLog)
        {
            var key = serviceName + ":IsWriteLog";

            if (configuration[key] == null)
            {
                return false;
            }

            try
            {
                SetValue<bool>(serviceName + ":IsWriteLog", isWriteLog);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion Methods
    }
}