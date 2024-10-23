namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.AffectedDownlineCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerClassificationAgencyService
    {
        Task<bool> AffectDownlineAgency(CustClassificationFromQueueModel agencyInfos, FromFlow fromFlow, string url);

        Task<bool> CallAPIPost<T>(T agencyInfos, int internalBatchSize, int externalBatchSize, FromFlow fromFlow, string url);
    }

    public class CustomerClassificationAgencyService : ICustomerClassificationAgencyService
    {
        private readonly ISentryClient _sentryClient;
        private const string ServiceName = "CustomerClassificationAgencyService";

        public CustomerClassificationAgencyService(ISentryClient sentryClient)
        {
            _sentryClient = sentryClient;
        }

        public async Task<bool> AffectDownlineAgency(CustClassificationFromQueueModel agencyInfos, FromFlow fromFlow, string url)
        {
            try
            {
                return await ApiHelper.Post<bool>(url, agencyInfos, 600);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, $" {fromFlow} - CTS.CC Service - Affect Downline Agency fail { url }", agencyInfos, ex, _sentryClient, "agencyInfos");

                return false;
            }
        }

        public async Task<bool> CallAPIPost<T>(T agencyInfos, int internalBatchSize, int externalBatchSize, FromFlow fromFlow, string url)
        {
            var request = new CustomerClassificationAgencyParameterModel<T>
            {
                Data = agencyInfos,
                InternalBatchSize = internalBatchSize,
                ExternalBatchSize = externalBatchSize
            };

            try
            {
                return await ApiHelper.Post<bool>(url, request, 600);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, $" {fromFlow} - CTS.CC Service - CallAPIPost fail { url }", request, ex, _sentryClient, JsonConvert.SerializeObject(request));

                return false;
            }
        }
    }
}
