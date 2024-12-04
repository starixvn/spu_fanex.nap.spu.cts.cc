namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface INormalDangerousAssociationScanService
    {
        ScanDangerousAssociationModel ScanDangerousAssociation(int batchSize);

        void InsertDangerousAssociationDataSource(IEnumerable<long> custIds);

        void CompleteDangerousAssociation(long lastScannedID);

        IEnumerable<long> CheckDangerousAssociationPerformance(IEnumerable<long> custIds);
    }

    public class NormalDangerousAssociationScanService : INormalDangerousAssociationScanService
    {
        private readonly INormalDangerousAssociationScanDataAccess _dataAccess;

        public NormalDangerousAssociationScanService(INormalDangerousAssociationScanDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IEnumerable<long> CheckDangerousAssociationPerformance(IEnumerable<long> custIds)
        {
            if (custIds.IsNotNullOrEmpty())
            {
                return _dataAccess.CheckDangerousAssociationPerformance(string.Join(", ", custIds));
            }

            return Enumerable.Empty<long>();
        }

        public void CompleteDangerousAssociation(long lastScannedID)
        {
            _dataAccess.CompleteDangerousAssociation(lastScannedID);
        }

        public void InsertDangerousAssociationDataSource(IEnumerable<long> custIds)
        {
            if (custIds.IsNotNullOrEmpty())
            {
                _dataAccess.InsertDangerousAssociationDataSource(string.Join(", ", custIds));
            }
        }

        public ScanDangerousAssociationModel ScanDangerousAssociation(int batchSize)
        {
            return _dataAccess.ScanDangerousAssociation(batchSize);
        }
    }
}
