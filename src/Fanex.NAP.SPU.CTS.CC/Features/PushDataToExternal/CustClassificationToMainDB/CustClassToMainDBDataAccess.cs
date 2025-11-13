namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface ICustClassToMainDBDataAccess
    {
        IEnumerable<LatestCCDangerLevelEntity> GetLatestCCAndDangerLevel(IEnumerable<long> custIds);

        DBResult UpdateCustomerClassification(IEnumerable<LatestClassificationEntity> custClassification, RoleGroupType roleGroup);

        DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelEntity> dangerLevel, int dangerTypeId);

        IEnumerable<GetCustomerClassificationBySportEntity> GetCustomerClassificationBySports(IEnumerable<BaseCustomerInfoBySportModel> custInfos);

        DBResult UpdateCustomerClassificationBySport(IEnumerable<CustomerClassificationUpdateBySportToMainDBEntity> custClassification);
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

        public DBResult UpdateCustomerClassification(IEnumerable<LatestClassificationEntity> custClassification, RoleGroupType roleGroup)
        {
            var criteria = new CustomerClassificationUpdateCriteria
            {
                Mode = (int)roleGroup,
                CustList = custClassification.ToXmlString()
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

                return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        public IEnumerable<GetCustomerClassificationBySportEntity> GetCustomerClassificationBySports(IEnumerable<BaseCustomerInfoBySportModel> custInfos)
        {
            var criteria = new GetCustomerClassificationBySportCriteria
            {
                CustInfos = JsonConvert.SerializeObject(custInfos)
            };

            return repository.Fetch<GetCustomerClassificationBySportEntity>(criteria) ?? Enumerable.Empty<GetCustomerClassificationBySportEntity>();
        }

        public DBResult UpdateCustomerClassificationBySport(IEnumerable<CustomerClassificationUpdateBySportToMainDBEntity> custClassification)
        {
            var criteria = new CustomerClassificationUpdateBySportCriteria
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
    }
}