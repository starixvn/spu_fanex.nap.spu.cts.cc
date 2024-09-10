namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartService
{
    using Fanex.Data.Repository;

    public interface ITWInitialSmartDataAccess
    {
        TWInitialSmartModel GetCustInitialSmart();
    }

    public class TWInitialSmartDataAccess : ITWInitialSmartDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWInitialSmartDataAccess()
        {
            repository = new DynamicRepository();
        }

        public TWInitialSmartModel GetCustInitialSmart()
        {
            var criteria = new TWInitialSmartGetCriteria();

            return new TWInitialSmartModel
            {
                CustInfos = repository.Fetch<TWInitialSmartEntity>(criteria),
                NextScannedDateTime = criteria.NextScannedDateTime,
                NextScannedCustID = criteria.NextScannedCustID
            };           
        }
    }
}
