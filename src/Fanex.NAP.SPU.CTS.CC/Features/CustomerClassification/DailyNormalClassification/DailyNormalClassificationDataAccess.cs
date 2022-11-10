namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyNormalClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IDailyNormalClassificationDataAccess
    {
        NormalClassificationDailyEntity GetCustomerForDailyNormalScanning();

        void ScanningDailyNormalClassification(string customers, FunctionId funcId);

        IEnumerable<RescanCategoryNormalEntity> GetExistingCustClassificationNormal(string custIDList);
    }

    public class DailyNormalClassificationDataAccess : IDailyNormalClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyNormalClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public NormalClassificationDailyEntity GetCustomerForDailyNormalScanning()
        {
            var criteria = new NormalClassificationForDailyScanningGetCriteria();

            var result = repository.Fetch<Int64>(criteria);

            return new NormalClassificationDailyEntity
            {
                CustID = result ?? Enumerable.Empty<Int64>(),
                op_LastCategoryID = criteria.LastCategoryID,
                op_LastCustID = criteria.LastCustID
            };
        }

        public void ScanningDailyNormalClassification(string customers, FunctionId funcId)
        {
            if (!string.IsNullOrEmpty(customers))
            {
                var criteria = new NormalAccountsClassificationGeneralCategorizeDailyCriteria
                {
                    CustomersXML = customers,               
                    FunctionId = (int)funcId
                };

                repository.Execute(criteria);
            }
        }

        public IEnumerable<RescanCategoryNormalEntity> GetExistingCustClassificationNormal(string custIDList)
        {
            var criteria = new GetExistingCustClassificationNormalCriteria
            {
                CustIDList = custIDList
            };

            return repository.Fetch<RescanCategoryNormalEntity>(criteria);
        }
    }
}