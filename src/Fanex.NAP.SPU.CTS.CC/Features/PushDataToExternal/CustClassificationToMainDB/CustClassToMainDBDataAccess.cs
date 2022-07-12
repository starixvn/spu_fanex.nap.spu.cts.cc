namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICustClassToMainDBDataAccess
    {
        IEnumerable<LatestCCDangerLevelEntity> GetLatestCCAndDangerLevel(IEnumerable<long> custIds);

        DBResult UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<LatestClassificationEntity> custClassification);

        DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevel, int dangerTypeId);
    }

    public class CustClassToMainDBDataAccess : ICustClassToMainDBDataAccess
    {
        private readonly IDynamicRepository repository;

        public CustClassToMainDBDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<LatestCCDangerLevelEntity> GetLatestCCAndDangerLevel(IEnumerable<long> custIds)
        {
            var criteria = new GetLastestCCDangerLevelCriteria
            {
                CustIds = string.Join(",", custIds)
            };

            return repository.Fetch<LatestCCDangerLevelEntity>(criteria) ?? Enumerable.Empty<LatestCCDangerLevelEntity>();
        }

        public DBResult UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<LatestClassificationEntity> custClassification)
        {
            var criteria = new CustomerClassificationUpdateCriteria
            {
                custList = custClassification.ToXmlString()
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = criteria.ErrorCode,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevel, int dangerTypeId)
        {
            if (dangerLevel != null && dangerLevel.Any())
            {
                var criteria = new DangerLevelUpdateCriteria
                {
                    CustList = dangerLevel.ToXmlString(),
                    DangerType = dangerTypeId
                };

                repository.Execute(criteria);

                LogService.Info($"UpdateDangerLevelToMainDB testing dangerLevels {JsonConvert.SerializeObject(dangerLevel)}");

                return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }
    }
}