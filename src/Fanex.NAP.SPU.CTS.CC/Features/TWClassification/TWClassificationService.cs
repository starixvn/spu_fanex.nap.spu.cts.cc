namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ITWClassificationService
    {
        IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize);
        Task<bool> SendDataToTW();

        void PreprocessSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs);

        void CompleteSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs);
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
        public TWClassificationService(ITWClassificationDataAccess twClassificationDataAccess)
        {
            this._twClassificationDataAccess = twClassificationDataAccess;
        }

        public async Task<bool> SendDataToTW()
        {
            var customers = _twClassificationDataAccess.GetSpecialLicSubCCs();
          
            foreach (var customer in customers)
            {
                var isSuccess = await RetryPushToTWApi(customer, RetryPushCCToExternal);

                if (!isSuccess)
                {
                    _twClassificationDataAccess.UpdateCustomerResponseStatus(
                        customer.ID,
                        (int)TWClassification.ProcessStatus_ResponseFail,
                        (int)TWClassification.HttpFailCode); 
                }
            }

            return true;
        }

        #region Private methods
        private async Task<bool> RetryPushToTWApi(TWCustomerResponse customer, int maxRetries)
        {
            // delays for retry attempts (10s for 1st retry, 30s for 2nd retry)
            var delays = new[] { 10000, 30000 };
            var attempt = 0;

            while (attempt <= maxRetries)
            {
                var responseCode = await FakePostApi(customer);

                if (responseCode == (int)TWClassification.HttpSuccessCode)
                {
                    _twClassificationDataAccess.UpdateCustomerResponseStatus(
                        customer.ID,
                        (int)TWClassification.ProcessStatus_ResponseComplete,
                        responseCode);

                    return true;
                }

                if (attempt == maxRetries)
                {
                    return false;
                }

                await Task.Delay(delays[Math.Min(attempt, delays.Length - 1)]);
                attempt++;
            }

            return false;
        }

        private static async Task<int> FakePostApi(TWCustomerResponse data)
        {
            await Task.Delay(100);
            return (int)TWClassification.HttpSuccessCode;
        }
        #endregion
    }
}
