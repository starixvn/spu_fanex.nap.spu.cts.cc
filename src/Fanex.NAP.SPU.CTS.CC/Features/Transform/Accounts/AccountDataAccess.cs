using Fanex.Data.Repository;
using Fanex.NAP.SPU.CTS.Features.Transform.Accounts.Criteria;
using Fanex.NAP.SPU.CTS.Features.Transform.Accounts.Entities;
using Fanex.NAP.SPU.CTS.Shares;
using System.Collections.Generic;

namespace Fanex.NAP.SPU.CTS.Features.Transform.Accounts
{
    public interface IAccountDataAccess
    {
        DataResult TransformDCSAccount(string accountJson);

        IEnumerable<AccountEntity> GetRawAccount(int numOfAccount, int numOfBatch);

        void ReTranformAccount(string jsonData, long? lastCustID);
    }

    public class AccountDataAccess : IAccountDataAccess
    {
        readonly IDynamicRepository repository;

        public AccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<AccountEntity> GetRawAccount(int numOfAccount, int numOfBatch)
        {
            var criteria = new TransformToCTSAccountGetPackageCriteria
            {
                NumberOfAccount = numOfAccount,
                NumberOfBatch = numOfBatch
            };

            return repository.Fetch<AccountEntity>(criteria);
        }

        public DataResult TransformDCSAccount(string accountJson)
        {
            var criteria = new TransformDCSAccountToCTSCustomerCriteria
            {
                AccountJson = accountJson
            };

           var errorCode =  repository.Execute(criteria);

            return new DataResult
            {
                ErrorCode = errorCode
            };
        }

        public void ReTranformAccount(string jsonData, long? lastCustID)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            var repository = new DynamicRepository();

            var criteria = new ReTransformAccountCriteria
            {
                CustomerInfo = jsonData,
                LastInsertCustID = lastCustID
            };

            repository.Execute(criteria);
        }
    }
}
