namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification
{
    using Fanex.Data.Repository;
    using System;
    using System.Linq;

    public interface IDailyNormalClassificationDataAccess
    {
        DailyNormalClassificationEntity GetCustomerForDailyNormalScanning();

        void ClassifyDailyNormal(string custIds);
    }

    public class DailyNormalClassificationDataAccess : IDailyNormalClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyNormalClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public DailyNormalClassificationEntity GetCustomerForDailyNormalScanning()
        {
            var criteria = new DailyNormalScanningGetCriteria();

            var result = repository.Fetch<Int64>(criteria);

            return new DailyNormalClassificationEntity
            {
                CustID = result ?? Enumerable.Empty<Int64>(),
                op_LastCategoryID = criteria.LastCategoryID,
                op_LastCustID = criteria.LastCustID
            };
        }

        public void ClassifyDailyNormal(string custIds)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var criteria = new DailyClassifyCriteria
                {
                    ListCustId = custIds
                };

                repository.Execute(criteria);
            }
        }
    }
}