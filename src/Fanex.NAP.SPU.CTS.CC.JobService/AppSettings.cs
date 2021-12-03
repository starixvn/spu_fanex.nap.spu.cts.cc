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

        string AFCInsertAccountsAPI { get; }

        string AFCDeleteAccountsAPI { get; }

        bool ProblemAccountIsWriteLog { get; }

        bool NormalAccountIsWriteLog { get; }

        bool RobotAccountIsWriteLog { get; }

        bool RemoveAccountEvidenceIsWriteLog { get; }

        bool CustomerEvidenceIsSendMailToSPU { get; }

        int CustomerEvidenceMailingToSPU { get; }

        bool TransformCTSCustomerIsWriteLog { get; }

        int TransformCTSCustomerBatchSize { get; }

        bool MonitorInsertNewCustIsAutoInsertMissingCust { get; }

        string SentryDsn { get; }

        bool UpdateIsWriteLogSetting(string serviceName, bool isWriteLog);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string environment;
        private readonly string evironmentName;
        private readonly string spName;
        private readonly string soon88FeedDataAPI;
        private readonly string afcInsertAccountsAPI;
        private readonly string afcDeleteAccountsAPI;
        private readonly string sentryDsn;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            environment = GetValue<string>(nameof(Environment));
            evironmentName = GetValue<string>(nameof(Environment));
            spName = GetValue<string>(nameof(SPName));
            soon88FeedDataAPI = GetValue<string>(nameof(Soon88FeedDataAPI));
            afcInsertAccountsAPI = GetValue<string>(nameof(AFCInsertAccountsAPI));
            afcDeleteAccountsAPI = GetValue<string>(nameof(AFCDeleteAccountsAPI));
            sentryDsn = GetValue<string>("Sentry:Dsn");
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

        public string AFCInsertAccountsAPI => afcInsertAccountsAPI;

        public string AFCDeleteAccountsAPI => afcDeleteAccountsAPI;

        public string SentryDsn => sentryDsn;

        public bool ProblemAccountIsWriteLog => GetValue<bool>("ProblemAccountInsert:IsWriteLog");

        public bool NormalAccountIsWriteLog => GetValue<bool>("NormalAccountInsert:IsWriteLog");

        public bool RobotAccountIsWriteLog => GetValue<bool>("RobotAccountInsert:IsWriteLog");

        public bool RemoveAccountEvidenceIsWriteLog => GetValue<bool>("RemoveAccountEvidence:IsWriteLog");

        public bool CustomerEvidenceIsSendMailToSPU => GetValue<bool>("CustomerEvidenceInsert:IsSendMailToSPU");

        public int CustomerEvidenceMailingToSPU => GetValue<int>("CustomerEvidenceInsert:MailingIdToSPU");

        public bool TransformCTSCustomerIsWriteLog => GetValue<bool>("TransformCTSCustomer:IsWriteLog");

        public int TransformCTSCustomerBatchSize => GetValue<int>("TransformCTSCustomer:BatchSize");

        public bool MonitorInsertNewCustIsAutoInsertMissingCust => GetValue<bool>("MonitorInsertNewCustomer:IsAutoInsertMissingCust");

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