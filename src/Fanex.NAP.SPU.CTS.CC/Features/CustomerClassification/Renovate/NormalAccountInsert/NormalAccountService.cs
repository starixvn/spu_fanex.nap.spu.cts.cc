namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalAccountService
    {
        GetNormalAccountModel GetNormalAccounts(string serviceName, bool IsWriteLog);

        IEnumerable<TWTaggedCustEntity> GetGroupBettingAndRejectCustomer(IEnumerable<GetNormalAccountEntity> custs);

        IEnumerable<CTSTaggedCustEntity> GetCustomerAssociationProblemAccount(string custIdList);

        InsertNormalAccountResultEntity InsertNormalAccount(IEnumerable<InsertNormalAccountModel> custs, string serviceName, bool isWriteLog);

        bool UpdateCompleteNormalAccountInsert(string custIdList, bool isSuccess);

        IEnumerable<NormalAccountAfterCheckTWDetectionModel> MergeWithTWTaggedCust(IEnumerable<GetNormalAccountEntity> custs, IEnumerable<TWTaggedCustEntity> twTaggedCust);

        IEnumerable<InsertNormalAccountModel> MergeWithCTSTaggedCust(IEnumerable<NormalAccountAfterCheckTWDetectionModel> custs, IEnumerable<CTSTaggedCustEntity> ctsTaggedCust);

    }

    public class NormalAccountService : INormalAccountService
    {
        private readonly INormalAccountDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public NormalAccountService(INormalAccountDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public GetNormalAccountModel GetNormalAccounts(string serviceName, bool IsWriteLog)
        {            
            var data = dataAccess.GetNormalAccount();

            Utilities.LogSentryInfo(serviceName, "Get Normal Accounts", data, IsWriteLog, sentryClient);

            return new GetNormalAccountModel
            {
                NormalAccount = data,
                IsContinute = data != null && data.Any()
            };
        }

        public IEnumerable<CTSTaggedCustEntity> GetCustomerAssociationProblemAccount(string custIdList)
        {
            return dataAccess.GetCustsAssociationProblemAccount(custIdList);           
        }

        public IEnumerable<TWTaggedCustEntity> GetGroupBettingAndRejectCustomer(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs != null && custs.Any())
            {
                var custIds = string.Join(",", custs.Select(x => x.CustId).Distinct());

                return dataAccess.CheckGroupBettingAndRejectCustomer(custIds);
            }

            return new List<TWTaggedCustEntity>();
        }

        public InsertNormalAccountResultEntity InsertNormalAccount(IEnumerable<InsertNormalAccountModel> custs, string serviceName, bool isWriteLog)
        {
            var result = new InsertNormalAccountResultEntity();

            var custsJson = JsonConvert.SerializeObject(custs);

            if (!string.IsNullOrEmpty(custsJson))
            {
                result = dataAccess.InsertNormalAccount(custsJson);

                Utilities.LogSentryInfo(serviceName, "InsertNormalAccount done", custsJson, isWriteLog, sentryClient);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Utilities.LogSentryError(serviceName, "InsertNormalAccount throws Exception", custsJson, new Exception(result.ErrorMessage), sentryClient);
                }
            }

            return result;
        }

        public bool UpdateCompleteNormalAccountInsert(string custIdList, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.UpdateCompleteNormalAccountInsert(custIdList);
            }            

            return true;
        }

        public IEnumerable<NormalAccountAfterCheckTWDetectionModel> MergeWithTWTaggedCust(IEnumerable<GetNormalAccountEntity> custs, IEnumerable<TWTaggedCustEntity> twTaggedCust)
        {
            if (custs == null || !custs.Any())
            {
                if (twTaggedCust == null || !twTaggedCust.Any())
                {
                    return custs.Select(x => new NormalAccountAfterCheckTWDetectionModel(x));
                }

                return new List<NormalAccountAfterCheckTWDetectionModel>();
            }

            return custs.GroupJoin(twTaggedCust,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new NormalAccountAfterCheckTWDetectionModel(group.custClass, temp));
        }

        public IEnumerable<InsertNormalAccountModel> MergeWithCTSTaggedCust(IEnumerable<NormalAccountAfterCheckTWDetectionModel> custs, IEnumerable<CTSTaggedCustEntity> ctsTaggedCust)
        {
            if (custs == null || !custs.Any())
            {
                if (ctsTaggedCust == null || !ctsTaggedCust.Any())
                {
                    return custs.Select(x => new InsertNormalAccountModel(x));
                }

                return new List<InsertNormalAccountModel>();
            }

            return custs.GroupJoin(ctsTaggedCust,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new InsertNormalAccountModel(group.custClass, temp));
        }
    }
}
