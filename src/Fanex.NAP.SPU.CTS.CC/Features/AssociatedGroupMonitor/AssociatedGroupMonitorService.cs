namespace Fanex.NAP.SPU.CTS.CC.Features.AssociatedGroupMonitor
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProblemAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares.Models;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IAssociatedGroupMonitorService
    {
        ScanAssociatedAccountModel ScanAssociatedAccount();

        DBResult AddAssociatedAccount(AddAssociatedAccountModel model);

        IEnumerable<CustomerInfoMainDBEntity> GetCustomerInfoMainDB(string custIdList, int sourceType);
        
        DBResult UpdateABIToMainDB(IEnumerable<CustomerInfoMainDBEntity> customerInfoMainDBs, int valueABI);

        DBResult UpdateOriToMainDB(IEnumerable<CustomerInfoMainDBEntity> customerInfoMainDBs, int valueOri);

        IEnumerable<long> InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> pas, PushDataToExternalEntity paras);
    }

    public class AssociatedGroupMonitorService : ProblemAccountServices, IAssociatedGroupMonitorService
    {
        private readonly IAssociatedGroupMonitorDataAccess dataAccess;
        private readonly ICustClassToMainDBService custClassToMainDBService;     

        public AssociatedGroupMonitorService(IBaseCustomerClassificationDataAccess baseDataAccess,
            IProblemAccountDataAccess problemAccountDataAccess,
            ICustClassToMainDBService custClassToMainDBService,
            IAssociatedGroupMonitorDataAccess dataAccess) : base(baseDataAccess, problemAccountDataAccess)
        {
            this.dataAccess = dataAccess;
            this.custClassToMainDBService = custClassToMainDBService;         
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

        public DBResult UpdateABIToMainDB(IEnumerable<CustomerInfoMainDBEntity> customerInfoMainDBs, int valueABI)
        {          
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

        public DBResult UpdateOriToMainDB(IEnumerable<CustomerInfoMainDBEntity> customerInfoMainDBs, int valueOri)
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

            return new DBResult { ErrorCode = 0 };
        }

        public IEnumerable<long> InsertProblemAccount(IEnumerable<InsertProblemAccountEntity> pas, PushDataToExternalEntity paras)
        {
            var paCustIDs = InsertProblemAccountPackage(pas, FunctionId.ScanAssociatedAccount, CCInputFlow.InsertPACategory, false, paras.ServiceName);

            custClassToMainDBService.PushExternalPackageByCustId(paCustIDs, paras);

            return paCustIDs;
        }
    }
}
