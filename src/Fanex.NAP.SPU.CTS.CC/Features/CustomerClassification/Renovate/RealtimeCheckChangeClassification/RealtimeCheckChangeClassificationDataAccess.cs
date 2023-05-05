namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RealtimeCheckChangeClassification
{
    using Fanex.Data.Repository;
    using System; 

    public interface IRealtimeCheckChangeClassificationDataAccess
    {
        RealtimeCheckChangeModel CheckChange();

        void UpdateLastTransDate(string custInfos);

        void UpdateNextScanCheckChange(DateTime? nextScanned);
        
    }

    public class RealtimeCheckChangeClassificationDataAccess : IRealtimeCheckChangeClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public RealtimeCheckChangeClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public RealtimeCheckChangeModel CheckChange() 
        {
            var criteria = new RealtimeCheckChangeCriteria();

            var data = repository.Fetch<RealtimeCheckChangeEntities>(criteria);

            return new RealtimeCheckChangeModel
            {
                CustInfos = data,
                NextScannedDateTime = criteria.NextScannedDateTime
            };
        }

        public void UpdateLastTransDate(string custInfos)
        {
            var criteria = new UpdateAssociationCustomerLastTransCriteria
            {
                CustInfos = custInfos
            };

            repository.Execute(criteria);
        }

        public void UpdateNextScanCheckChange(DateTime? nextScanned)
        {
            var criteria = new RealtimeCheckChangeUpdateNextScanCriteria
            {
                NextScannedDateTime = nextScanned
            };

            repository.Execute(criteria);
        }
        
    }
}
