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
        GetNormalAccountModel GetNormalAccounts();

        InsertNormalAccountResultEntity InsertNormalAccount(IEnumerable<InsertNormalAccountModel> custs, string serviceName, bool isWriteLog);

        bool UpdateCompleteNormalAccountInsert(string custIdList, long scannedMaxId, bool isSuccess);

        IEnumerable<InsertNormalAccountModel> MergeWithTWTaggedCustAndCTSTaggedCust(IEnumerable<GetNormalAccountEntity> custs);

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

        public GetNormalAccountModel GetNormalAccounts()
        {
            return dataAccess.GetNormalAccount();
        }

        public InsertNormalAccountResultEntity InsertNormalAccount(IEnumerable<InsertNormalAccountModel> custs, string serviceName, bool isWriteLog)
        {
            var result = new InsertNormalAccountResultEntity();

            var custsJson = custs != null && custs.Any() ? JsonConvert.SerializeObject(custs) : string.Empty;

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

        public bool UpdateCompleteNormalAccountInsert(string custIdList, long scannedMaxId, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.UpdateCompleteNormalAccountInsert(custIdList, scannedMaxId);
            }

            return true;
        }

        public IEnumerable<InsertNormalAccountModel> MergeWithTWTaggedCustAndCTSTaggedCust(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs == null || !custs.Any())
            {
                return Enumerable.Empty<InsertNormalAccountModel>();
            }

            var twTaggedCusts = GetGroupBettingAndRejectCustomer(custs.Where(x => x.IsCheckTaggingTW));

            var custAfterCheckTWTagged = MergeWithTWTaggedCust(custs, twTaggedCusts);

            var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

            var finalNormalAcc = MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            return finalNormalAcc;
        }

        #region private method
        private IEnumerable<CTSTaggedCustEntity> GetCustomerAssociationProblemAccount(string custIdList)
        {
            return dataAccess.GetCustsAssociationProblemAccount(custIdList);
        }

        private IEnumerable<TWTaggedCustEntity> GetGroupBettingAndRejectCustomer(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs != null && custs.Any())
            {
                var custIds = string.Join(",", custs.Select(x => x.CustId).Distinct());

                return dataAccess.CheckGroupBettingAndRejectCustomer(custIds);
            }

            return new List<TWTaggedCustEntity>();
        }

        private IEnumerable<NormalAccountAfterCheckTWDetectionModel> MergeWithTWTaggedCust(IEnumerable<GetNormalAccountEntity> custs, IEnumerable<TWTaggedCustEntity> twTaggedCust)
        {
            var custsMapping = custs ?? new List<GetNormalAccountEntity>();
            var twTaggedCustMapping = twTaggedCust ?? new List<TWTaggedCustEntity>();

            return custsMapping.GroupJoin(twTaggedCustMapping,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new NormalAccountAfterCheckTWDetectionModel(group.custClass, temp));
        }

        private IEnumerable<InsertNormalAccountModel> MergeWithCTSTaggedCust(IEnumerable<NormalAccountAfterCheckTWDetectionModel> custs, IEnumerable<CTSTaggedCustEntity> ctsTaggedCust)
        {
            var custsMapping = custs ?? new List<NormalAccountAfterCheckTWDetectionModel>();
            var ctsTaggedCustMapping = ctsTaggedCust ?? new List<CTSTaggedCustEntity>();

            return custsMapping.GroupJoin(ctsTaggedCustMapping,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new InsertNormalAccountModel(group.custClass, temp));
        }
        #endregion
    }
}
