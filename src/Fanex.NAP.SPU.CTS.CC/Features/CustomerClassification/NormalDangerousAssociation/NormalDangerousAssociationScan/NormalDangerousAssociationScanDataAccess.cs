namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Model;
    using System.Collections.Generic;

    public interface INormalDangerousAssociationScanDataAccess
    {
        ScanDangerousAssociationModel ScanDangerousAssociation(int batchSize);

        void InsertDangerousAssociationDataSource(string custIdList);

        void CompleteDangerousAssociation(long lastScannedID);

        IEnumerable<long> CheckDangerousAssociationPerformance(string custIdList);
    }

    public class NormalDangerousAssociationScanDataAccess : INormalDangerousAssociationScanDataAccess
    {
        private readonly IDynamicRepository _repository;

        public NormalDangerousAssociationScanDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public ScanDangerousAssociationModel ScanDangerousAssociation(int batchSize)
        {
            var criteria = new ScanDangerousAssociationCriteria
            {
                BatchSize = batchSize
            };

            return new ScanDangerousAssociationModel
            {
                CustIds = _repository.Fetch<long>(criteria),
                LastScannedID = criteria.LastScannedID,
            };
        }

        public void InsertDangerousAssociationDataSource(string custIdList)
        {
            var criteria = new InsertDangerousAssociationDataSourceCriteria
            {
                CustIDList = custIdList
            };

            _repository.Execute(criteria);
        }

        public void CompleteDangerousAssociation(long lastScannedID)
        {
            var criteria = new CompleteDangerousAssociationCriteria
            {
                LastScannedID = lastScannedID
            };

            _repository.Execute(criteria);
        }

        public IEnumerable<long> CheckDangerousAssociationPerformance(string custIdList)
        {
            var criteria = new CheckDangerousAssociationPerformanceCriteria
            {
                ListCustId = custIdList
            };

            return _repository.Fetch<long>(criteria);
        }
    }
}
