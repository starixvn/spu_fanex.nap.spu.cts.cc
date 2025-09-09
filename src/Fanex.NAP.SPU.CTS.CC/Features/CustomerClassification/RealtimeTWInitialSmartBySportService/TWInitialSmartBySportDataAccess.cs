namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Model;

    public interface ITWInitialSmartBySportDataAccess
    {
        TWInitialSmartBySportModel GetCustInitialSmartBySport(int internalBatchSize);
    }

    public class TWInitialSmartBySportDataAccess : ITWInitialSmartBySportDataAccess
    {
        private readonly IDynamicRepository _repository;

        public TWInitialSmartBySportDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public TWInitialSmartBySportModel GetCustInitialSmartBySport(int internalBatchSize)
        {
            var criteria = new TWInitialSmartGetBySportCriteria
            {
                InternalBatchSize = internalBatchSize,
            };

            return new TWInitialSmartBySportModel
            {
                CustInfos = _repository.Fetch<TWInitialSmartBySportEntity>(criteria),
                NextScannedCustID = criteria.NextScannedCustID,
                NextScannedDateTime = criteria.NextScannedDateTime,
            };
        }
    }
}
