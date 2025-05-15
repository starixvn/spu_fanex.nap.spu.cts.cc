namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using System;
    using System.Threading.Tasks;
    using Entities;

    public interface ITWClassificationService
    {
        Task<bool> SendDataToTW();
    }

    public class TWClassificationService : ITWClassificationService
    {
        private readonly ITWClassificationDataAccess _twClassificationDataAccess;

        public TWClassificationService(ITWClassificationDataAccess twClassificationDataAccess)
        {
            this._twClassificationDataAccess = twClassificationDataAccess;
        }

        public async Task<bool> SendDataToTW()
        {
            var customers = _twClassificationDataAccess.GetSpecialLicSubCCs();
            foreach (var customer in customers)
            {
                var retries = 0;
                var responseCode = 0;
                var isSuccess = false;
                int[] delays = { 10000, 30000 }; // 10s, 30s

                while (retries < 3 && !isSuccess)
                {
                    responseCode = await FakePostApi(customer);
                    if (responseCode == 200)
                    {
                        isSuccess = true;
                        _twClassificationDataAccess.UpdateCustomerResponseStatus(customer.ID, 4, responseCode); // 4: ResponseComplete
                    }
                    else
                    {
                        retries++;
                        if (retries < 3)
                        {
                            await Task.Delay(delays[Math.Min(retries - 1, delays.Length - 1)]);
                        }
                    }
                }

                if (!isSuccess)
                {
                    _twClassificationDataAccess.UpdateCustomerResponseStatus(customer.ID, 3, responseCode); // 3: ResponseFail
                }
            }
            return true;
        }

        private static async Task<int> FakePostApi(TWCustomerResponse data)
        {
            await Task.Delay(100);
            return 200;
        }
    }
}
