namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification.Criteria;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface IRealtimeClassificationDataAccess
    {
        IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryHasChangeForRunRealtime();

        IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryForRunRealtime(string custSportJson);

        void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustClassificationRealtimeEntity> customers);

        void RealtimeUpdateLastScanTime(bool isGeneralCategory);

        void UpdateAssociationCustomerLastTicket(string custIdList);
    }

    public class RealtimeClassificationDataAccess : IRealtimeClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public RealtimeClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryHasChangeForRunRealtime()
        {
            var criteria = new CustClassificationHasChangesForRealtimeScanningGetCriteria();

            return repository.Fetch<CustClassificationRealtimeEntity>(criteria);
        }

        public IEnumerable<CustClassificationRealtimeEntity> GetCustomerGeneralCategoryForRunRealtime(string custSportJson)
        {
            var criteria = new CustClassificationByCategoryForRealtimeScanningGetCriteria
            {
                CustSportsJson = custSportJson
            };

            return repository.Fetch<CustClassificationRealtimeEntity>(criteria);
        }

        public void ClassifyCustomerGeneralCategoryRealtime(IEnumerable<CustClassificationRealtimeEntity> customers)
        {
            var criteria = new NormalAccountsClassificationGeneralCategorizeRealtimeCriteria
            {
                CustomersXML = customers.ToXmlString()
            };

            repository.Execute(criteria);
        }

        public void RealtimeUpdateLastScanTime(bool isGeneralCategory)
        {
            var criteria = new RealtimeLastScanTimeUpdateCriteria
            {
                IsGeneralCategory = isGeneralCategory
            };

            repository.Execute(criteria);
        }

        public void UpdateAssociationCustomerLastTicket(string custIdList)
        {
            var criteria = new UpdateAssociationCustomerLastTicketCriteria
            {
                CustIdList = custIdList
            };

            repository.Execute(criteria);
        }
    }
}