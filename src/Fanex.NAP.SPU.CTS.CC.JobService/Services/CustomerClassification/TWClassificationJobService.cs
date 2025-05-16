namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Sentry;
    using System;
    using System.Threading.Tasks;
    using Features.TWClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Linq;
    using Newtonsoft.Json;
    using Shares.Models;
    using System.Collections.Generic;
    using Features.TWClassification.Models;
    using System.Net;

    public interface ITWClassificationJobService
    {
        Task<bool> ScanTWSpecialLicSubCC(int batchInternalSize);

        Task<bool> TWResponseHaifa(int batchSize);
    }

    public class TWClassificationJobService : ITWClassificationJobService
    {
        private readonly string _serviceName;
        private readonly ISentryClient _sentryClient;        
        private readonly ITWClassificationService _twClassificationService;
        private readonly IAppSettings _appSettings;

        public TWClassificationJobService(ISentryClient sentryClient,          
            ITWClassificationService twClassificationService,
            IAppSettings appSettings)
        {            
            _sentryClient = sentryClient;            
            _twClassificationService = twClassificationService;
            _appSettings = appSettings;
            _serviceName = "TWClassification";
        }

        public Task<bool> ScanTWSpecialLicSubCC(int batchInternalSize)
        {
            try
            {
                bool isContinue;

                do
                {
                    var custLicSubs = _twClassificationService.GetSpecialLicSubCCScan(batchInternalSize);

                    isContinue = custLicSubs != null && custLicSubs.Any();

                    if (isContinue)
                    {
                        _twClassificationService.PreprocessSpecialLicSubCCScan(custLicSubs);

                        _twClassificationService.CompleteSpecialLicSubCCScan(custLicSubs);
                    }
                } while (isContinue);                
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(_serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public async Task<bool> TWResponseHaifa(int batchSize)
        {
            try
            {
                if (batchSize <= 0)
                {
                    batchSize = 1000;
                }

                var customers = _twClassificationService.GetSpecialLicSubCCResponse(batchSize);
                var completeResponses = new List<SpecialLicSubCCResponseCompleteModel>();

                foreach (var customer in customers)
                {
                    var request = new SpecialLicSubCCRequestModel
                    {
                        CustID = customer.CustID,
                        SpecialCustomerClass = customer.APICustomerClass,
                        ResponseDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                        ErrorCode = customer.StatusCode,
                        ErrorMessage = customer.ItemNameDisplay,
                        HttpStatusCode = (int)HttpStatusCode.OK
                    };

                    var response = await SendSpecialLicSubCCToTWApi(request);

                    completeResponses.Add(new SpecialLicSubCCResponseCompleteModel
                    {
                        ID = customer.ID,
                        ResponseStatus = response.IsSuccess ? 1 : 0,
                        ResponseHttpCode = (int)response.StatusCode,
                        ResponseTime = request.ResponseDateTime
                    });
                }

                var responseJson = JsonConvert.SerializeObject(completeResponses);

                _twClassificationService.CompleteSpecialLicSubCCResponse(responseJson);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(_serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        #region Private Methods
        private async Task<APIHelperModel> SendSpecialLicSubCCToTWApi(SpecialLicSubCCRequestModel request)
        {
            //return await ApiHelper.PostWithResponseHttpCode(string.Empty, request);
            return new APIHelperModel
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK
            };
        }
        #endregion Private Methods
    }
}
