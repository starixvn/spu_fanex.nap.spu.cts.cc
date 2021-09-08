namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyClassification
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyClassificationServices
    {
        IEnumerable<CustClassificationDailyEntity> GetCustClassificationForDailyScanning(int batchQuantity, int rowQuantityPerBatch);

        void ScanningDailyClassification(IEnumerable<CustClassificationDailyEntity> customers, FunctionId funcId, bool isCheckLast14Days);

        void ClassifyCustomerGeneralCategoryDaily(IEnumerable<CustClassificationDailyEntity> customers, FunctionId funcId, bool isCheckLast14Days);

        void ClassifyCustomerGeneralCategoryDaily(IEnumerable<long> customers, FunctionId funcId, bool isCheckLast14Days);

        void UpdateDailyClassificationStatus(IEnumerable<CustClassificationDailyEntity> customers);
    }

    public class DailyClassificationServices : IDailyClassificationServices
    {
        private readonly IDailyClassificationDataAccess dailyClassificationDataAccess;

        public DailyClassificationServices(IDailyClassificationDataAccess dailyClassificationDataAccess)
        {
            this.dailyClassificationDataAccess = dailyClassificationDataAccess;
        }

        public IEnumerable<CustClassificationDailyEntity> GetCustClassificationForDailyScanning(int batchQuantity, int rowQuantityPerBatch)
        {
            return dailyClassificationDataAccess.GetCustomerForDailyScanning(batchQuantity, rowQuantityPerBatch);
        }

        public void ScanningDailyClassification(IEnumerable<CustClassificationDailyEntity> customers, FunctionId funcId, bool isCheckLast14Days)
        {
            if (customers != null && customers.Any())
            {
                var custGroupByBatch = customers.GroupBy(c => c.BatchID);

                foreach (var custGroup in custGroupByBatch)
                {
                    ClassifyCustomerGeneralCategoryDaily(custGroup, funcId, true);
                    UpdateDailyClassificationStatus(custGroup);
                }
            }
        }

        public void ClassifyCustomerGeneralCategoryDaily(IEnumerable<CustClassificationDailyEntity> customers, FunctionId funcId, bool isCheckLast14Days)
        {
            if (customers != null && customers.Any())
            {
                var custIds = string.Join(",", customers.Select(c => c.CustId));

                dailyClassificationDataAccess.ClassifyCustomerGeneralCategoryDaily(custIds, funcId, isCheckLast14Days);
            }
        }

        public void ClassifyCustomerGeneralCategoryDaily(IEnumerable<long> customers, FunctionId funcId, bool isCheckLast14Days)
        {
            if (customers != null && customers.Any())
            {
                var custIdToString = string.Join(",", customers);

                dailyClassificationDataAccess.ClassifyCustomerGeneralCategoryDaily(custIdToString, funcId, isCheckLast14Days);
            }
        }

        public void UpdateDailyClassificationStatus(IEnumerable<CustClassificationDailyEntity> customers)
        {
            if (customers != null && customers.Any())
            {
                string jsonData = JsonConvert.SerializeObject(customers.Select(a => new
                {
                    a.CustId,
                    a.SportGroupId
                }));

                dailyClassificationDataAccess.ClassifyCustomerDailyDone(jsonData);
            }
        }
    }
}