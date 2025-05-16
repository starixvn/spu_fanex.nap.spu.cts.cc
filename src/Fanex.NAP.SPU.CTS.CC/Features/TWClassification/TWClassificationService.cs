namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Net.Http.Json;
    using Fanex.NAP.SPU.CTS.CC.Features.TWClassification.Models;

    public interface ITWClassificationService
    {
        IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize);        

        void PreprocessSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs);

        void CompleteSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs);

        Task<bool> SendDataToTW(int batchSize);
    }

    public class TWClassificationService : ITWClassificationService
    {
        private readonly ITWClassificationDataAccess _twClassificationDataAccess;


        public TWClassificationService(ITWClassificationDataAccess twClassificationDataAccess)
        {
            this._twClassificationDataAccess = twClassificationDataAccess;
        }

        public IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize)
        {
            return _twClassificationDataAccess.GetSpecialLicSubCCScan(batchSize);
        }

        public void PreprocessSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs)
        {
            if (custLicSubs.Any())
            {
                var custIdList = string.Join(",", custLicSubs.Select(x => x.CustID).Distinct());

                _twClassificationDataAccess.PreprocessSpecialLicSubCCScan(custIdList);
            }
        }

        public void CompleteSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs)
        {
            if (custLicSubs.Any())
            {
                var ids = string.Join(",", custLicSubs.Select(x => x.ID));

                _twClassificationDataAccess.CompleteSpecialLicSubCCScan(ids);
            }            
        }
      
        public async Task<bool> SendDataToTW(int batchSize)
        {
            var customers = _twClassificationDataAccess.GetSpecialLicSubCCResponse(batchSize);
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
                    HttpStatusCode = 200
                };

                var response = await CallApiTW(request);

                completeResponses.Add(new SpecialLicSubCCResponseCompleteModel
                {
                    ID = customer.ID,
                    ResponseStatus = response.ErrorCode,
                    ResponseHttpCode = response.HttpStatusCode,
                    ResponseTime = request.ResponseDateTime
                });
            }

            var responseJson = JsonConvert.SerializeObject(completeResponses);
            _twClassificationDataAccess.CompleteSpecialLicSubCCResponse(responseJson);

            return true;
        }

        #region Private method
        private async Task<SpecialLicSubCCRequestModel> CallApiTW(SpecialLicSubCCRequestModel request)
        {
            request.HttpStatusCode = 200;

            return request;
        }
        #endregion
    }
}
