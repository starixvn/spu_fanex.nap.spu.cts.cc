namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalTaggingClassificationServices
    {
        public INormalTaggingClassificationServices Start();

        public INormalTaggingClassificationServices GetNormalTaggingScanning(int batchExternalSize);

        public INormalTaggingClassificationServices PreprocessCustomerClassNormalTagging(int batchInternalSize);

        public bool UpdateLastScanningNormalTaggingParameter();
    }

    public class NormalTaggingClassificationServices : INormalTaggingClassificationServices
    {
        private readonly INormalTaggingClassificationDataAccess _normalTaggingClassificationDataAccess;
        private readonly ISystemParameterDataAccess _systemParameterDataAccess;
        private readonly INormalAccountService _normalAccountService;
        private GetNormalTaggingScanningModel _getNormalTaggingScanningModel;
        private IEnumerable<long> _custIDs;

        public NormalTaggingClassificationServices(INormalTaggingClassificationDataAccess normalTaggingClassificationDataAccess, ISystemParameterDataAccess systemParameterDataAccess, INormalAccountService normalAccountService)
        {
            _normalTaggingClassificationDataAccess = normalTaggingClassificationDataAccess;
            _systemParameterDataAccess = systemParameterDataAccess;
            _normalAccountService = normalAccountService;
        }

        public INormalTaggingClassificationServices Start()
        {
            _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel();
            _custIDs = new List<long>();

            return this;
        }

        public INormalTaggingClassificationServices GetNormalTaggingScanning(int batchExternalSize)
        {
            _getNormalTaggingScanningModel = this._normalTaggingClassificationDataAccess.GetNormalTaggingScanning(batchExternalSize);
            _custIDs = _getNormalTaggingScanningModel?.CustIDs;

            return this;
        }

        public INormalTaggingClassificationServices PreprocessCustomerClassNormalTagging(int batchInternalSize)
        {
            if (_custIDs != null && _custIDs.Any())
            {
                var custIdChunks = _custIDs?.Chunk(batchInternalSize);

                foreach (var custBatch in custIdChunks)
                {
                    var custBatchCheckChanges = CheckChangesNormalTaggingScanning(custBatch);

                    var custIdList = string.Join(",", custBatchCheckChanges);

                    if (!string.IsNullOrEmpty(custIdList))
                    {
                        _normalTaggingClassificationDataAccess.PreprocessCustomerClassNormalTagging(custIdList);
                    }
                }
            }

            return this;
        }

        public bool UpdateLastScanningNormalTaggingParameter()
        {
            if (_custIDs != null && _custIDs.Any())
            {
                _systemParameterDataAccess.UpdateTimingExecution(_getNormalTaggingScanningModel.LastCategoryID.ToString(), SystemParameters.AssociatedGroupLastCTSCustID);
                _systemParameterDataAccess.UpdateTimingExecution(_getNormalTaggingScanningModel.LastCustID.ToString(), SystemParameters.AssociatedGroupLastCTSCustID);

                return true;
            }

            return false;
        }

        private IEnumerable<long> CheckChangesNormalTaggingScanning(IEnumerable<long> custBatch)
        {
            if (_custIDs != null && _custIDs.Any())
            {
                var convertNormalTaggingList = custBatch.Select(cust => new GetNormalAccountEntity { CustId = cust });

                var twTaggedCusts = _normalAccountService.GetGroupBettingAndRejectCustomer(convertNormalTaggingList);

                var custAfterCheckTWTagged = _normalAccountService.MergeWithTWTaggedCust(convertNormalTaggingList, twTaggedCusts);

                var custIdAfterCheckTWTagged = string.Join(",", custAfterCheckTWTagged.Where(x => x.TaggingID == 0).Select(x => x.CustId).Distinct());

                var custAssWithPA = _normalAccountService.GetCustomerAssociationProblemAccount(custIdAfterCheckTWTagged);

                var finalNormalAcc = _normalAccountService.MergeWithCTSTaggedCust(custAfterCheckTWTagged, custAssWithPA);

                return _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(finalNormalAcc.Select(x => new CheckChangesNormalTaggingScanningModel
                {
                    CustID = x.CustId,
                    TaggingID = x.TaggingID,
                    TaggingType = x.TaggingType
                }));
            }

            return Enumerable.Empty<long>();
        }
    }
}
