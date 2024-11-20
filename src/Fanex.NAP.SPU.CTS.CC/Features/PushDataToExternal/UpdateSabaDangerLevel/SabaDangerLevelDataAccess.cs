namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.UpdateSabaDangerLevel
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface ISabaDangerLevelDataAccess
    {
        SabaDangerLevelResultEntity GetSabaDangerLevelForScanning();

        SabaDangerLevelInitResultEntity GetSabaDangerLevelForInitialization();

        void UpdateLogSabaDangerLevel(string custInfoJson);

        SabaDangerLevelUpdateResultEntity UpdateDabaDangerLevelToMainDB(IEnumerable<SabaDangerLevelEntity> custSabaDangerLevel);
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
                LastHistoryID = criteria.LastHistoryID,
                AgencyLastHistoryID = criteria.AgencyLastHistoryID
            };
        }

        public SabaDangerLevelInitResultEntity GetSabaDangerLevelForInitialization()
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

        public SabaDangerLevelUpdateResultEntity UpdateDabaDangerLevelToMainDB(IEnumerable<SabaDangerLevelEntity> custSabaDangerLevel)
        {
            var criteria = new SabaDangerLevelUpdateCriteria
            {
                CustList = custSabaDangerLevel.ToXmlString()
            };

            repository.Execute(criteria);

            return new SabaDangerLevelUpdateResultEntity
            {
                ErrCode = criteria.ErrCode,
                ErrMsg = criteria.ErrMsg,
                IsSuccess = string.IsNullOrEmpty(criteria.ErrMsg) && criteria.ErrCode == 0
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