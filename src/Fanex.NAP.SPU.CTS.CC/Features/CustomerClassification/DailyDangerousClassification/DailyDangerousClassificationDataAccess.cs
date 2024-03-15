namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousScoreClassification
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;

    public interface IDailyDangerousClassificationDataAccess
    {
        IEnumerable<DangerousDetectionToCCEntity> GetDangerousScoreToCC(DateTime scannedDate);

        IEnumerable<CheckIsDangerousDetectionByCustEntity> CheckIsDangerousByCust(int CustID);
    }

    public class DailyDangerousClassificationDataAccess : IDailyDangerousClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public DailyDangerousClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<DangerousDetectionToCCEntity> GetDangerousScoreToCC(DateTime scannedDate)
        {
            var criteria = new GetDangerousDetectionToCCCriteria
            {
                ScannedDate = scannedDate
            };

            return repository.Fetch<DangerousDetectionToCCEntity>(criteria);
        }

        public IEnumerable<CheckIsDangerousDetectionByCustEntity> CheckIsDangerousByCust(int CustID)
        {
            var criteria = new CheckIsDangerousDetectionByCustCriteria
            {
                CustID = CustID
            };

            return repository.Fetch<CheckIsDangerousDetectionByCustEntity>(criteria);
        }

    }
}
