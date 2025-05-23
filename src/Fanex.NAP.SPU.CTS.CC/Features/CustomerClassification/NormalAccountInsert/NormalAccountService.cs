namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Model;
    using Fanex.NAP.SPU.CTS.CC.Shares;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using static Fanex.NAP.SPU.CTS.Shares.IEnumerableExtention;

    public interface INormalAccountService
    {
        GetNormalAccountModel GetNormalAccounts(bool isLogData);

        ResultInsertCustomerClassificationModel InsertNormalAccount(IEnumerable<InsertNormalAccountEntity> custs, string serviceName, bool isWriteLog);

        void CompleteNormalAccountInsert(long scannedMaxId, bool isSuccess);

        IEnumerable<InsertNormalAccountEntity> MergeWithTaggedNormalTaggingFlow(IEnumerable<GetNormalAccountEntity> custs);
    }

    public class NormalAccountService : BaseCustomerClassificationService, INormalAccountService
    {
        private readonly INormalAccountDataAccess dataAccess;

        public NormalAccountService(IBaseCustomerClassificationDataAccess baseDataAccess, INormalAccountDataAccess dataAccess, ISentryClient sentryClient) : base(baseDataAccess, sentryClient)
        {
            this.dataAccess = dataAccess;
        }

        public GetNormalAccountModel GetNormalAccounts(bool isLogData)
        {
            return dataAccess.GetNormalAccount(isLogData);
        }

        public ResultInsertCustomerClassificationModel InsertNormalAccount(IEnumerable<InsertNormalAccountEntity> custs, string serviceName, bool isWriteLog)
        {
            var result = new ResultInsertCustomerClassificationModel();

            if (custs != null && custs.Any())
            {
                var modelInsert = new InsertCustomerClassificationModel
                {
                    FromAction = (int)FromFlow.CTSServices,
                    CustInfoJson = JsonConvert.SerializeObject(custs)
                };

                result = base.InsertCustomerClassification(CCInputFlow.Normal, modelInsert, serviceName);
            }

            return result;
        }

        public void CompleteNormalAccountInsert(long scannedMaxId, bool isSuccess)
        {
            if (isSuccess)
            {
                dataAccess.CompleteNormalAccountInsert(scannedMaxId);
            }
        }

        public IEnumerable<InsertNormalAccountEntity> MergeWithTaggedNormalTaggingFlow(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs.IsNullOrEmpty())
            {
                return Enumerable.Empty<InsertNormalAccountEntity>();
            }

            var processTaggingCusts = custs.Select(x => new ProcessTaggingModel(x)).ToList();

            var custAfterProcessTWTagged = ProcessTWTaggedCust(processTaggingCusts);

            var custAfterProcessAssWithPA = ProcessAssWithPACust(custAfterProcessTWTagged);

            var custAfterCheckTWTaggedSpecialLicSub = ProcessTWTaggedSpecialLicSubCust(custAfterProcessAssWithPA);

            var result = custAfterCheckTWTaggedSpecialLicSub.Select(x => new InsertNormalAccountEntity(x));
            return result;
        }

        #region private method

        private IEnumerable<ProcessTaggingModel> ProcessTWTaggedCust(IEnumerable<ProcessTaggingModel> custs)
        {
            var twTaggedCusts = GetGroupBettingAndRejectCustomer(custs.Where(x => x.ScanTaggingTWType == 1));

            var twTaggedExcludeModels = CreateExcludeModelFromTWTaggedAndCTSCust(custs, twTaggedCusts);
            var excludedCustIds = dataAccess.CheckTWTaggingExclude(twTaggedExcludeModels).Select(m => m.CustID);

            var hashSetExcludedCustIds = new HashSet<int>(excludedCustIds);
            var twTaggedMeetConditions = twTaggedCusts.Where(x => hashSetExcludedCustIds.Contains(x.CustID));

            var custAfterCheckTWTagged =
                MergeWithOtherEnumerable(custs, twTaggedMeetConditions, (source, other) => source.MergeTWTaggedCust(other));

            return custAfterCheckTWTagged;
        }

        private IEnumerable<ProcessTaggingModel> ProcessAssWithPACust(IEnumerable<ProcessTaggingModel> custAfterCheckTWTagged)
        {
            var custCheckAssWithPA = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = GetCustomerAssociationProblemAccount(custCheckAssWithPA);

            var custAfterCheckAssWithPA = MergeWithOtherEnumerable(custAfterCheckTWTagged, custAssWithPA, (source, other) => source.MergeCTSTaggedCust(other));

            return custAfterCheckAssWithPA;
        }

        private IEnumerable<ProcessTaggingModel> ProcessTWTaggedSpecialLicSubCust(IEnumerable<ProcessTaggingModel> custs)
        {
            var custCheckSpecialLicSub = custs.Where(x => x.TaggingID == 0).Select(x => new TWTaggedSpecialLicSubModel { CustID = x.CustId, ScanSpecialLicSubType = x.ScanSpecialLicSubType });

            var specialLicSubCusts = GetTWTaggedCustBySpecialLicSub(custCheckSpecialLicSub);

            var custAfterCheckTWTaggedSpecialLicSub =
                MergeWithOtherEnumerable(custs, specialLicSubCusts, (source, other) => source.MergeTWTaggedSpecialLicSubCust(other));

            return custAfterCheckTWTaggedSpecialLicSub;
        }

        /// <summary>
        /// Expect at momment, 1 cust has only 1 categoryid and 1 tw tagging
        /// </summary>
        private IEnumerable<TWTaggedExcludeModel> CreateExcludeModelFromTWTaggedAndCTSCust(IEnumerable<ProcessTaggingModel> ctsCusts, IEnumerable<TWTaggedCustEntity> twTaggedCusts)
        {
            return twTaggedCusts
                .Where(twCust => twCust.IsTWTaggedCustValid())
                .GroupJoin(ctsCusts,
                    twCust => twCust.CustID,
                    ctsCust => ctsCust.CustId,
                    (twCust, ctsCustGroup) => new { twCust, ctsCustGroup })
                .SelectMany(
                    x => x.ctsCustGroup.DefaultIfEmpty(),
                    (x, ctsCust) => new TWTaggedExcludeModel
                    {
                        CustID = x.twCust.CustID,
                        CategoryID = ctsCust?.CategoryId,
                        TaggingID = x.twCust.TaggingID
                    });
        }

        private IEnumerable<CTSTaggedCustEntity> GetCustomerAssociationProblemAccount(string custIdList)
        {
            return dataAccess.GetCustsAssociationProblemAccount(custIdList);
        }

        private IEnumerable<TWTaggedCustEntity> GetGroupBettingAndRejectCustomer(IEnumerable<ProcessTaggingModel> custs)
        {
            if (custs != null && custs.Any())
            {
                var custIds = string.Join(",", custs.Select(x => x.CustId).Distinct());

                return dataAccess.CheckGroupBettingAndRejectCustomer(custIds);
            }

            return new List<TWTaggedCustEntity>();
        }

        private IEnumerable<CTSTaggedCustEntity> GetTWTaggedCustBySpecialLicSub(IEnumerable<TWTaggedSpecialLicSubModel> custInfos)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var customerJson = JsonConvert.SerializeObject(custInfos);

                return dataAccess.GetTWTaggedCustBySpecialLicSub(customerJson);
            }

            return new List<CTSTaggedCustEntity>();
        }
        #endregion
    }
}