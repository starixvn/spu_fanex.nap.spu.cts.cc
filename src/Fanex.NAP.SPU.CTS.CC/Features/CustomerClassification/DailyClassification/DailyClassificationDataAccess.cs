namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyClassification
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyClassificationDataAccess
    {
        IEnumerable<CustClassificationDailyEntity> GetCustomerForDailyScanning(int batchQuantity, int rowQuantityPerBatch);

        void ClassifyCustomerGeneralCategoryDaily(string customers, FunctionId funcId, bool isCheckLast14Days = true);

        void ClassifyCustomerDailyDone(string custSportJson);
    }

    public class DailyClassificationDataAccess : IDailyClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustClassificationDailyEntity> GetCustomerForDailyScanning(int batchQuantity, int rowQuantityPerBatch)
        {
            var criteria = new CustClassificationForDailyScanningGetCriteria
            {
                BatchQuantity = batchQuantity,
                RecordQuantityPerBatch = rowQuantityPerBatch
            };

            return repository.Fetch<CustClassificationDailyEntity>(criteria);
        }

        public void ClassifyCustomerGeneralCategoryDaily(string customers, FunctionId funcId, bool isCheckLast14Days = true)
        {
            if (!string.IsNullOrEmpty(customers))
            {
                var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
                {
                    ListCustIds = customers,
                    IsCheckLast14Days = isCheckLast14Days,
                    FunctionId = (int)funcId
                };

                repository.Execute(criteria);
            }
        }

        public void ClassifyCustomerDailyDone(string custSportJson)
        {
            if (!string.IsNullOrEmpty(custSportJson))
            {
                var criteria = new CustClassificationForDailyScanningCompleteCriteria
                {
                    CustSportJson = custSportJson
                };

                repository.Execute(criteria);
            }
        }
    }
}