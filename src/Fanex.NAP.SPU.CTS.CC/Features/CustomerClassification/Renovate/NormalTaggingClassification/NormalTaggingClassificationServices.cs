namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalTaggingClassificationServices
    {
        public INormalTaggingClassificationServices Start();

        public INormalTaggingClassificationServices GetNormalTaggingScanning();

        public INormalTaggingClassificationServices PreprocessCustomerClassNormalTagging(int batchInternalSize);

        public bool UpdateLastScanningNormalTaggingParameter();
    }

    public class NormalTaggingClassificationServices : INormalTaggingClassificationServices
    {
        private readonly INormalTaggingClassificationDataAccess _normalTaggingClassificationDataAccess;
        private readonly ISystemParameterDataAccess _systemParameterDataAccess;
        private readonly INormalAccountService _normalAccountService;
        private GetNormalTaggingScanningModel _getNormalTaggingScanningModel;
        private IEnumerable<GetNormalTaggingScanningViewModel> _custInfos;

        public NormalTaggingClassificationServices(INormalTaggingClassificationDataAccess normalTaggingClassificationDataAccess, ISystemParameterDataAccess systemParameterDataAccess, INormalAccountService normalAccountService)
        {
            _normalTaggingClassificationDataAccess = normalTaggingClassificationDataAccess;
            _systemParameterDataAccess = systemParameterDataAccess;
            _normalAccountService = normalAccountService;
        }

        public INormalTaggingClassificationServices Start()
        {
            _getNormalTaggingScanningModel = new GetNormalTaggingScanningModel();
            _custInfos = new List<GetNormalTaggingScanningViewModel>();

            return this;
        }

        public INormalTaggingClassificationServices GetNormalTaggingScanning()
        {
            _getNormalTaggingScanningModel = this._normalTaggingClassificationDataAccess.GetNormalTaggingScanning();
            _custInfos = _getNormalTaggingScanningModel?.CustInfos;

            return this;
        }

        public INormalTaggingClassificationServices PreprocessCustomerClassNormalTagging(int batchInternalSize)
        {
            if (_custInfos != null && _custInfos.Any())
            {
                var custIdChunks = _custInfos?.Chunk(batchInternalSize);

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
            if (_getNormalTaggingScanningModel != null)
            {
                _systemParameterDataAccess.UpdateTimingExecution(_getNormalTaggingScanningModel.LastCategoryID.ToString(), SystemParameters.NormalTaggingClassificationLastCategoryID);
                _systemParameterDataAccess.UpdateTimingExecution(_getNormalTaggingScanningModel.LastCustID.ToString(), SystemParameters.NormalTaggingClassificationLastCustID);

                return _custInfos != null && _custInfos.Any();
            }

            return false;
        }

        private IEnumerable<long> CheckChangesNormalTaggingScanning(IEnumerable<GetNormalTaggingScanningViewModel> custBatch)
        {
            if (custBatch != null && custBatch.Any())
            {
                var convertNormalTaggingList = custBatch.Select(custInfo => new GetNormalAccountEntity { CustId = custInfo.CustID, ScanTaggingTWType = custInfo.IsCheckTaggingTW ? 1 : 0 });

                var finalNormalAcc = _normalAccountService.MergeWithTaggedNormalTaggingFlow(convertNormalTaggingList);

                var checkChangesNormalTaggingMapping = finalNormalAcc.Select(x => new CheckChangesNormalTaggingScanningModel
                {
                    CustID = x.CustId,
                    TaggingID = x.TaggingID,
                    TaggingType = x.TaggingType
                });

                if (checkChangesNormalTaggingMapping != null && checkChangesNormalTaggingMapping.Any())
                {
                    return _normalTaggingClassificationDataAccess.CheckChangesNormalTaggingScanning(JsonConvert.SerializeObject(checkChangesNormalTaggingMapping));
                }
            }

            return Enumerable.Empty<long>();
        }
    }
}
