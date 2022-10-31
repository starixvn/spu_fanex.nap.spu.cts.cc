namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections.Generic;
    using System.Linq;

    public interface IAssociatedGroupMonitorService
    {
        ScanAssociatedAccountModel ScanAssociatedAccount();

        DBResult AddAssociatedAccount(AddAssociatedAccountModel model);

        IEnumerable<CustomerInfoMainDBEntity> GetCustomerInfoMainDB(string custIdList, int sourceType);

        DBResult UpdateDangerLevelToMainDB(IEnumerable<CustomerInfoMainDBEntity> customerInfoMainDBs, int valueABI, int valueOri);
    }

    public class AssociatedGroupMonitorService : IAssociatedGroupMonitorService
    {
        private readonly IAssociatedGroupMonitorDataAccess dataAccess;

        public AssociatedGroupMonitorService(IAssociatedGroupMonitorDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public ScanAssociatedAccountModel ScanAssociatedAccount()
        {
            return dataAccess.ScanAssociatedAccount();
        }

        public DBResult AddAssociatedAccount(AddAssociatedAccountModel model)
        {
            return dataAccess.AddAssociatedAccount(model);
        }

        public IEnumerable<CustomerInfoMainDBEntity> GetCustomerInfoMainDB(string custIdList, int sourceType)
        {
            return dataAccess.GetCustomerInfoMainDB(custIdList, sourceType);
        }

        public DBResult UpdateDangerLevelToMainDB(IEnumerable<CustomerInfoMainDBEntity> customerInfoMainDBs, int valueABI, int valueOri)
        {
            if (valueOri > 0)
            {
                var custToUpdateOri = customerInfoMainDBs.Where(x => x.Danger1 == 0).Select(x => new DangerLevelModel
                {
                    CustId = x.CustID,
                    DangerLevel = valueOri
                });

                if (custToUpdateOri.Any())
                {
                    dataAccess.UpdateDangerLevelToMainDB(custToUpdateOri, 1);
                }
            }

            if (valueABI > 0)
            {
                var custToUpdateABI = customerInfoMainDBs.Where(x => x.Danger2 == 0).Select(x => new DangerLevelModel
                {
                    CustId = x.CustID,
                    DangerLevel = valueABI
                });

                if (custToUpdateABI.Any())
                {
                    dataAccess.UpdateDangerLevelToMainDB(custToUpdateABI, 2);
                }
            }           

            return new DBResult { ErrorCode = 0 };
        }
    }
}
