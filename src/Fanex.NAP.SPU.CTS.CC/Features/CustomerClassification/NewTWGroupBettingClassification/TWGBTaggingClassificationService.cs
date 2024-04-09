namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NewTWGroupBettingClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface ITWGBTaggingClassificationService
    {
        ITWGBTaggingClassificationService GetChanges();

        ITWGBTaggingClassificationService UpdateNextScannedTime();

        void PreprocessWithBatch(int batchSize);
    }

    public class TWGBTaggingClassificationService : ITWGBTaggingClassificationService
    {
        private readonly ITWGBTaggingClassificationDataAccess _twGBTaggingDataAccess;
        private readonly IRealtimeClassificationDataAccess _realtimeClassificationDataAccess;
        private TWGBTaggingCheckChangeModel _twGBTaggingCheckChange;

        public TWGBTaggingClassificationService(ITWGBTaggingClassificationDataAccess dataAccess, IRealtimeClassificationDataAccess realtimeClassificationDataAccess)
        {
            _twGBTaggingDataAccess = dataAccess;
            _realtimeClassificationDataAccess = realtimeClassificationDataAccess;
        }

        public ITWGBTaggingClassificationService GetChanges()
        {
            _twGBTaggingCheckChange = _twGBTaggingDataAccess.GetChanges();

            return this;
        }

        public ITWGBTaggingClassificationService UpdateNextScannedTime()
        {
            if (_twGBTaggingCheckChange != null)
            {
                _twGBTaggingDataAccess.UpdateNextScannedTime(_twGBTaggingCheckChange.NextScannedDateTime);
            }

            return this;
        }

        public void PreprocessWithBatch(int batchSize)
        {
            if (_twGBTaggingCheckChange?.CustIDs == null || !_twGBTaggingCheckChange.CustIDs.Any()) return;

            var customerChunks = _twGBTaggingCheckChange.CustIDs.Distinct().Chunk(batchSize);

            customerChunks.AsParallel().WithDegreeOfParallelism(5).ForAll(customers => Preprocess(customers));
        }

        private void Preprocess(IEnumerable<long> custIds)
        {
            if (custIds == null || !custIds.Any()) return;

            var getCustomerCategory = _realtimeClassificationDataAccess.GetCustomerCategory(string.Join(",", custIds));

            if (getCustomerCategory?.CustomerCategory != null && getCustomerCategory.CustomerCategory.Count() > 0)
            {
                _twGBTaggingDataAccess.Preprocess(string.Join(",", getCustomerCategory?.CustomerCategory));
            }
        }
    }
}
