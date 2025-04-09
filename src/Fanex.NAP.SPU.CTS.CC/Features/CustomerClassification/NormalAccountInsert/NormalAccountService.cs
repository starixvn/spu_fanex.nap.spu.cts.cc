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

            var custAfterProcessTWTagged = ProcessTWTaggedCust(custs);

            var custAfterProcessAssWithPA = ProcessAssWithPACust(custAfterProcessTWTagged);

            return custAfterProcessAssWithPA;
        }

        #region private method

        private IEnumerable<NormalAccountAfterCheckTWDetectionModel> ProcessTWTaggedCust(IEnumerable<GetNormalAccountEntity> custs)
        {
            var twTaggedCusts = GetGroupBettingAndRejectCustomer(custs.Where(x => x.ScanTaggingTWType == 1));

            var twTaggedExcludeModels = CreateExcludeModelFromTWTaggedAndCTSCust(custs, twTaggedCusts);
            var excludedCustIds = dataAccess.CheckTWTaggingExclude(twTaggedExcludeModels).Select(m => m.CustID);

            var hashSetExcludedCustIds = new HashSet<int>(excludedCustIds);
            var twTaggedMeetConditions = twTaggedCusts.Where(x => hashSetExcludedCustIds.Contains(x.CustID));

            var custAfterCheckTWTagged = MergeWithTWTaggedCust(custs, twTaggedMeetConditions);

            return custAfterCheckTWTagged;
        }

        private IEnumerable<InsertNormalAccountEntity> ProcessAssWithPACust(IEnumerable<NormalAccountAfterCheckTWDetectionModel> custAfterCheckTWTagged)
        {
            var custCheckAssWithPA = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

            var custAssWithPA = GetCustomerAssociationProblemAccount(custCheckAssWithPA);

            var custAfterCheckAssWithPA = MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

            return custAfterCheckAssWithPA;
        }

        /// <summary>
        /// Expect at momment, 1 cust has only 1 categoryid and 1 tw tagging
        /// </summary>
        private IEnumerable<TWTaggedExcludeModel> CreateExcludeModelFromTWTaggedAndCTSCust(IEnumerable<GetNormalAccountEntity> ctsCusts, IEnumerable<TWTaggedCustEntity> twTaggedCusts)
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

        private IEnumerable<TWTaggedCustEntity> GetGroupBettingAndRejectCustomer(IEnumerable<GetNormalAccountEntity> custs)
        {
            if (custs != null && custs.Any())
            {
                var custIds = string.Join(",", custs.Select(x => x.CustId).Distinct());

                return dataAccess.CheckGroupBettingAndRejectCustomer(custIds);
            }

            return new List<TWTaggedCustEntity>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "<Pending>")]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "<Pending>")]
        private IEnumerable<InsertNormalAccountEntity> MergeWithCTSTaggedCust(IEnumerable<NormalAccountAfterCheckTWDetectionModel> custs, IEnumerable<CTSTaggedCustEntity> ctsTaggedCust)
        {
            var custsMapping = custs ?? new List<NormalAccountAfterCheckTWDetectionModel>();
            var ctsTaggedCustMapping = ctsTaggedCust ?? new List<CTSTaggedCustEntity>();

            return custsMapping.GroupJoin(ctsTaggedCustMapping,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => new InsertNormalAccountEntity(group.custClass, temp));
        }

        #endregion
    }
}
