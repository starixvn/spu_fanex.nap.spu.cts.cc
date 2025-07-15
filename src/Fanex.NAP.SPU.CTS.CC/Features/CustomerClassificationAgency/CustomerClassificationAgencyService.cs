namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationAgency.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustomerClassificationAgencyService
    {
        Task<bool> CallAPIPost<T>(T agencyInfos, int internalBatchSize, int externalBatchSize, FromFlow fromFlow, string url);
    }

    public class CustomerClassificationAgencyService : ICustomerClassificationAgencyService
    {
        private const string ServiceName = "CustomerClassificationAgencyService";

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
                return await ApiHelper.Post<bool>(url, request, 1200);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(ServiceName, $" {fromFlow} - CTS.CC Service - Call AgencyCC API fail { url }", request, ex, JsonConvert.SerializeObject(request));

                return false;
            }
        }
    }
}
