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

        Task<bool> TWResponseHaifa(int batchSize, bool isResponse);
    }

    public class TWClassificationJobService : ITWClassificationJobService
    {
        private readonly string _serviceName;
        private readonly ISentryClient _sentryClient;        
        private readonly ITWClassificationService _twClassificationService;
        private readonly IAppSettings _appSettings;
        private const int HttpCodeNotCalled = 999;
        private const int DefaultBatchSize = 1000;

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

        public async Task<bool> TWResponseHaifa(int batchSize, bool isResponse)
        {
            try
            {
                var completeResponses = new List<SpecialLicSubCCResponseCompleteModel>();

                if (batchSize <= 0)
                {
                    batchSize = DefaultBatchSize;
                }
              
                var customers = _twClassificationService.GetSpecialLicSubCCResponse(batchSize);

                foreach (var customer in customers)
                {
                    var completeResponse = await ProcessCustomerAsync(customer, isResponse);

                    completeResponses.Add(completeResponse);
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
        private async Task<APIHelperModel> SendSpecialLicSubCCToTWApi(SpecialLicSubCCRequestModel request, bool isResponse)
        {
            if (isResponse)
            {
                return await ApiHelper.PostWithResponseHttpCode(_appSettings.TWResponseHaifaUrl, request);
            }
            
            return new APIHelperModel
            {
                IsSuccess = true
            };
        }

        private async Task<SpecialLicSubCCResponseCompleteModel> ProcessCustomerAsync(SpecialLicSubCCResponseGetEntity customer, bool isResponse)
        {
            var request = CreateSpecialLicSubCCRequest(customer);

            var response = await SendSpecialLicSubCCToTWApi(request, isResponse);

            return new SpecialLicSubCCResponseCompleteModel
            {
                ID = customer.ID,
                ResponseStatus = response.IsSuccess ? 1 : 0,
                ResponseHttpCode = isResponse ? (int)response.StatusCode : HttpCodeNotCalled,
                ResponseTime = request.ResponseDateTime
            };
        }

        private SpecialLicSubCCRequestModel CreateSpecialLicSubCCRequest(SpecialLicSubCCResponseGetEntity customer)
        {
            return new SpecialLicSubCCRequestModel
            {
                CustID = customer.CustID,
                SpecialCustomerClass = customer.LatestCustomerClass,
                ResponseDateTime = DateTime.Now.ToString(Constants.CONVERT_TO_SQL_DATETIME),
                ErrorCode = customer.StatusCode,
                ErrorMessage = customer.ItemNameDisplay,
                HttpStatusCode = (int)HttpStatusCode.OK
            };
        }
        #endregion Private Methods
    }
}
