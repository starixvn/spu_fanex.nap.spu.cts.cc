namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IAssociatedGroupMonitorDataAccess
    {
        ScanAssociatedAccountModel ScanAssociatedAccount();

        DBResult AddAssociatedAccount(AddAssociatedAccountModel model);

        IEnumerable<CustomerInfoMainDBEntity> GetCustomerInfoMainDB(string custIdList, int sourceType);

        DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelModel> dangerLevel, int dangerType);
    }

    public class AssociatedGroupMonitorDataAccess : IAssociatedGroupMonitorDataAccess
    {
        private readonly IDynamicRepository repository;

        public AssociatedGroupMonitorDataAccess()
        {
            repository = new DynamicRepository();
        }

        public ScanAssociatedAccountModel ScanAssociatedAccount()
        {
            var criteria = new ScanAssociatedAccountCriteria();

            var data = repository.Fetch<ScanAssociatedAccountEntity>(criteria);

            return new ScanAssociatedAccountModel
            {
                GroupID = criteria.GroupID,
                GroupName = criteria.GroupName,
                ABI = criteria.ABI,
                LastCTSCustID = criteria.LastCTSCustID,
                ScanAssociatedAccountList = data
            };
        }

        public DBResult AddAssociatedAccount(AddAssociatedAccountModel model)
        {
            var criteria = new AddAssociatedAccountCriteria
            {
                GroupID = model.GroupID,
                CTSCustIDs = model.CTSCustIDs,
                IsAuto = model.IsAuto,
                Remark = model.Comment,
                CreatedBy = model.CreatedBy
            };

            try
            {
                repository.Execute(criteria);

                return new DBResult { ErrorCode = 0 };
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Add associated account Exception { logMessage }", ex);

                return new DBResult { ErrorCode = -1, ErrorMsg = ex.Message };
            }
        }

        public IEnumerable<CustomerInfoMainDBEntity> GetCustomerInfoMainDB(string custIdList, int sourceType)
        {
            try
            {
                var criteria = new CustomerInfoMainDBCriteria
                {
                    CustIdList = custIdList,
                    SourceType = sourceType
                };

                return repository.Fetch<CustomerInfoMainDBEntity>(criteria);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                 .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                 .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"Get Customer Info MainDB Exception { logMessage } custIdList: {custIdList } sourceType: {sourceType}", ex);
            }

            return Enumerable.Empty<CustomerInfoMainDBEntity>();
        }

        public DBResult UpdateDangerLevelToMainDB(IEnumerable<DangerLevelModel> dangerLevel, int dangerType)
        {
            if (dangerLevel != null && dangerLevel.Any())
            {
                var criteria = new DangerLevelUpdateCriteria
                {
                    CustList = dangerLevel.ToXmlString(),
                    DangerType = dangerType
                };

                repository.Execute(criteria);

                return new DBResult { ErrorCode = criteria.ErrorCode, ErrorMsg = criteria.ErrorMessage };
            }

            return new DBResult { ErrorCode = 0, ErrorMsg = string.Empty };
        }

    }
}
