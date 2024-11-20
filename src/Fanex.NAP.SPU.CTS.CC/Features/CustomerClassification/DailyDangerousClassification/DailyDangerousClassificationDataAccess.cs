namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification
{
    using Fanex.Data.Repository;
    using System.Linq;

    public interface IDailyDangerousClassificationDataAccess
    {
        DangerousToCCModel GetDangerousClassification(bool isLicensee);        
    }

    public class DailyDangerousClassificationDataAccess : IDailyDangerousClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyDangerousClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public DangerousToCCModel GetDangerousClassification(bool isLicensee)
        {
            var criteria = new GetDangerousDetectionToCCCriteria
            {
                IsLicensee = isLicensee
            };

            var (firstResultSet, secondResultSet) = repository.FetchMultiple<DangerousToCCLastScannedEntity, DangerousToCCEntity>(criteria);

            return new DangerousToCCModel
            {
                DangerousCCLastScanned = firstResultSet.SingleOrDefault(),
                DangerousCCs = secondResultSet
            };
        }
      
    }
}
