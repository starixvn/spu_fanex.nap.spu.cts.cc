namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.Data.Repository;
    using System.Linq;

    public interface ISabaDangerLevelDataAccess
    {
        SabaDangerLevelResultEntity GetSabaDangerLevelForScanning();

        SabaDangerLevelInitResultEntity GetSabaDangerLevelForInitializing();

        void UpdateLogSabaDangerLevel(string custInfoJson);
    }

    public class SabaDangerLevelDataAccess : ISabaDangerLevelDataAccess
    {
        private readonly IDynamicRepository repository;

        public SabaDangerLevelDataAccess()
        {
            repository = new DynamicRepository();
        }

        public SabaDangerLevelResultEntity GetSabaDangerLevelForScanning()
        {
            var criteria = new SabaDangerLevelScanCriteria();

            var result = repository.Fetch<SabaDangerLevelEntity>(criteria);

            return new SabaDangerLevelResultEntity
            {
                CustSabaDangerLevel = result ?? Enumerable.Empty<SabaDangerLevelEntity>(),
                LastHistoryID = criteria.LastHistoryID
            };
        }

        public SabaDangerLevelInitResultEntity GetSabaDangerLevelForInitializing()
        {
            var criteria = new SabaDangerLevelInitGetCriteria();

            var result = repository.Fetch<SabaDangerLevelEntity>(criteria);

            return new SabaDangerLevelInitResultEntity
            {
                CustSabaDangerLevel = result ?? Enumerable.Empty<SabaDangerLevelEntity>(),
                LastCategoryID = criteria.LastCategoryID,
                LastCustID = criteria.LastCustID
            };
        }

        public void UpdateLogSabaDangerLevel(string custInfoJson)
        {
            var criteria = new SabaDangerLevelLogUpdateCriteria
            {
                CustInfo = custInfoJson
            };

            repository.Execute(criteria);
        }
    }
}