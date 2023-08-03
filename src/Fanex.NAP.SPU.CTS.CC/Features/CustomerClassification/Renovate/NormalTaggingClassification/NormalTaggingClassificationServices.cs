namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalTaggingClassificationServices
    {
        public INormalTaggingClassificationServices Start();

        public INormalTaggingClassificationServices GetCustIdsForNormalTaggingScanning(int batchExternalSize);

        public bool PreprocessCustomerClassNormalTagging(int batchInternalSize);
    }

    public class NormalTaggingClassificationServices : INormalTaggingClassificationServices
    {
        private readonly INormalTaggingClassificationDataAccess _dataAccess;
        private IEnumerable<long> taggingCustIds;

        public NormalTaggingClassificationServices(INormalTaggingClassificationDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public INormalTaggingClassificationServices Start()
        {
            taggingCustIds = new List<long>();

            return this;
        }

        public INormalTaggingClassificationServices GetCustIdsForNormalTaggingScanning(int batchExternalSize)
        {
            taggingCustIds = this._dataAccess.GetTaggingCustIds(batchExternalSize);

            return this;
        }

        public bool PreprocessCustomerClassNormalTagging(int batchInternalSize)
        {
            if (taggingCustIds != null && taggingCustIds.Any())
            {
                var taggingCustIdChunks = taggingCustIds.Chunk(batchInternalSize);

                foreach (var custBatch in taggingCustIdChunks)
                {
                    var custIdList = string.Join(",", custBatch);

                    _dataAccess.PreprocessCustomerClassNormalTagging(custIdList);
                }

                return true;
            }

            return false;
        }
    }
}
