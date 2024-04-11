namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountService
    {
        GetNormalAccountModel GetNormalAccounts();

        InsertNormalAccountResultEntity InsertNormalAccount(IEnumerable<InsertNormalAccountModel> custs, string serviceName, bool isWriteLog);

        bool CompleteNormalAccountInsert(long scannedMaxId, bool isSuccess);

        IEnumerable<InsertNormalAccountModel> MergeWithTaggedNormalTaggingFlow(IEnumerable<GetNormalAccountEntity> custs);

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

        public bool CompleteNormalAccountInsert(long scannedMaxId, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.CompleteNormalAccountInsert(scannedMaxId);
            }

            return true;
        }

        public IEnumerable<InsertNormalAccountModel> MergeWithTaggedNormalTaggingFlow(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs == null || !custs.Any())
            {
                return Enumerable.Empty<InsertNormalAccountModel>();
            }

            var twTaggedCusts = GetGroupBettingAndRejectCustomer(custs.Where(x => x.ScanTaggingTWType == 1));

            var custAfterCheckTWTagged = MergeWithTWTaggedCust(custs, twTaggedCusts);

            var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

            var finalNormalAcc = MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            return finalNormalAcc;
        }

        public IEnumerable<InsertNormalAccountModel> MergeWithTWTaggedCustAndCTSTaggedCust(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs == null || !custs.Any())
            {
                return Enumerable.Empty<InsertNormalAccountModel>();
            }

            var twGBTaggingNewCusts = default(IEnumerable<NewTWTaggedCustEntity>);
            var twTaggingSRPCusts = default(IEnumerable<TWTaggedCustEntity>);

            var tasks = new Task[]
            {
                Task.Run(() => twGBTaggingNewCusts = GetNewTWGroupBetting(custs.Where(x => x.ScanTaggingTWType == 2))),
                Task.Run(() => twTaggingSRPCusts = GetGroupBettingAndRejectCustomer(custs.Where(x => x.ScanTaggingTWType == 1)))
            };

            Task.WhenAll(tasks).Wait();

            var custAfterCheckTWTagged = MergeWithTWTaggedCustAndNewTWCust(custs, twTaggingSRPCusts, twGBTaggingNewCusts);

            var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

            var finalNormalAcc = MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            return finalNormalAcc;
        }

        #region private method
        private IEnumerable<NewTWTaggedCustEntity> GetNewTWGroupBetting(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs != null && custs.Any())
            {
                var custIds = string.Join(",", custs.Select(x => x.CustId).Distinct());

                return dataAccess.CheckNewGroupBetting(custIds);
            }

            return new List<NewTWTaggedCustEntity>();
        }

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

        private IEnumerable<NormalAccountAfterCheckTWDetectionModel> MergeWithTWTaggedCustAndNewTWCust(IEnumerable<GetNormalAccountEntity> custs, IEnumerable<TWTaggedCustEntity> twTaggingSRPCusts, IEnumerable<NewTWTaggedCustEntity> twGBTaggingNewCusts)
        {
            var custsMapping = custs ?? new List<GetNormalAccountEntity>();
            var twTaggingSRPCustMapping = twTaggingSRPCusts ?? new List<TWTaggedCustEntity>();
            var twGBTaggingNewCustMapping = twGBTaggingNewCusts ?? new List<NewTWTaggedCustEntity>();

            var result = from cust in custsMapping
                         join srpCust in twTaggingSRPCustMapping on cust.CustId equals srpCust.CustID into srpCust_JoinTable
                         from srpCustDefault in srpCust_JoinTable.DefaultIfEmpty()
                         join newCust in twGBTaggingNewCustMapping on cust.CustId equals newCust.CustID into newCust_JoinTable
                         from newCustDefault in newCust_JoinTable.DefaultIfEmpty()
                         select MappingNormalAccountAfterCheckTWDetectionModel(cust, srpCustDefault, newCustDefault);

            return result;
        }

        private NormalAccountAfterCheckTWDetectionModel MappingNormalAccountAfterCheckTWDetectionModel(GetNormalAccountEntity cust, TWTaggedCustEntity twTaggingSRPCust, NewTWTaggedCustEntity twGBTaggingNewCust)
        {
            if (twTaggingSRPCust != null)
            {
                return new NormalAccountAfterCheckTWDetectionModel(cust, twTaggingSRPCust);
            }

            if (twGBTaggingNewCust != null)
            {
                return new NormalAccountAfterCheckTWDetectionModel(cust, twGBTaggingNewCust);
            }

            return new NormalAccountAfterCheckTWDetectionModel(cust);
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
