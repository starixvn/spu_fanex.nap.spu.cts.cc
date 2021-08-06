namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustClassToMainDBDataAccess
    {
        IEnumerable<LatestClassificationEntity> GetLatestClassification(string custIds);

        DBResult UpdateCustomerGeneralClassificationtoMainDB(IEnumerable<LatestClassificationEntity> custClassification);

        DBResult UpdateAppliedCustomerClassification(string problemCustomerJson);

        IEnumerable<DangerLevelEntity> GetDangerLevel(string custIds, DangerLevel typeId);

        DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevel, DangerLevel typeId);

        void UpdateAppliedDangerLevel(string custsJson, DangerLevel typeId);

        DBResult LogInputData(IEnumerable<LogInputDataEntity> content, FunctionId funcId);
    }

    public class CustClassToMainDBDataAccess : ICustClassToMainDBDataAccess
    {
        private readonly IDynamicRepository repository;

        public CustClassToMainDBDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<LatestClassificationEntity> GetLatestClassification(string custIds)
        {
            var criteria = new LastestClassificationGetCriteria
            {
                CustIds = custIds
            };

            return repository.Fetch<LatestClassificationEntity>(criteria);
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

        public DBResult UpdateAppliedCustomerClassification(string problemCustomerJson)
        {
            var criteria = new AppliedCustomerClassificationUpdateCriteria
            {
                ProblemCustomerJson = problemCustomerJson
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = !string.IsNullOrEmpty(criteria.ErrorMessage) ? -1 : 0,
                ErrorMsg = criteria.ErrorMessage
            };
        }

        public IEnumerable<DangerLevelEntity> GetDangerLevel(string custIds, DangerLevel typeId)
        {
            if (!string.IsNullOrEmpty(custIds))
            {
                var criteria = new DangerLevelGetCriteria
                {
                    CustIds = custIds,
                    TypeID = (int)typeId
                };

                return repository.Fetch<DangerLevelEntity>(criteria);
            }

            return new List<DangerLevelEntity>();
        }

        public DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevel, DangerLevel typeId)
        {
            if (dangerLevel != null && dangerLevel.Any())
            {
                var criteria = new DangerLevelUpdateCriteria
                {
                    CustList = dangerLevel.ToXmlString(),
                    DangerType = (int)typeId
                };

                repository.Execute(criteria);

                return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public void UpdateAppliedDangerLevel(string custsJson, DangerLevel typeId)
        {
            if (!string.IsNullOrEmpty(custsJson))
            {
                var criteria = new AppliedDangerLevelUpdateCriteria
                {
                    CustJson = custsJson,
                    TypeID = (int)typeId
                };

                repository.Execute(criteria);
            }
        }

        public DBResult LogInputData(IEnumerable<LogInputDataEntity> content, FunctionId funcId)
        {
            var criteria = new LogInputDataCriteria
            {
                InputData = JsonConvert.SerializeObject(content),
                FunctionID = (int)funcId
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : 1,
                ErrorMsg = criteria.ErrorMessage
            };
        }
    }
}