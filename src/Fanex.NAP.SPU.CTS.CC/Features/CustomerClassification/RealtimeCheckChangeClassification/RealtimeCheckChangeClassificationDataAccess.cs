namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeCheckChangeClassification
{
    using Fanex.Data.Repository;

    public interface IRealtimeCheckChangeClassificationDataAccess
    {
        RealtimeCheckChangeModel CheckChange();

        void UpdateLastTransDate(string custInfos);

        void UpdateLastTransDateAgency(string custTranDatesAgency);

        void UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned);
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

            var (firstResultSet, secondResultSet) = repository.FetchMultiple<RealtimeCheckChangeEntities, RealtimeCheckChangeAgencyEntities>(criteria);

            return new RealtimeCheckChangeModel
            {
                CustInfos = firstResultSet,
                CustInfosAgency = secondResultSet,
                NextScannedDateTime1 = criteria.NextScannedDateTime1,
                NextScannedDateTime2 = criteria.NextScannedDateTime2,
                NextScannedDateTime3 = criteria.NextScannedDateTime3,
                NextScannedDateTime4 = criteria.NextScannedDateTime4
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

        public void UpdateLastTransDateAgency(string custTranDatesAgency)
        {
            var criteria = new UpdateAssociationCustomerLastTransAgencyCriteria
            {
                CustTranDatesAgency = custTranDatesAgency
            };

            repository.Execute(criteria);
        }

        public void UpdateNextScanCheckChange(RealtimeCheckChangeModel nextScanned)
        {
            var criteria = new RealtimeCheckChangeUpdateNextScanCriteria
            {
                NextScannedDateTime1 = nextScanned.NextScannedDateTime1,
                NextScannedDateTime2 = nextScanned.NextScannedDateTime2,
                NextScannedDateTime3 = nextScanned.NextScannedDateTime3,
                NextScannedDateTime4 = nextScanned.NextScannedDateTime4,
            };

            repository.Execute(criteria);
        }
    }
}