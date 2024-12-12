namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface INormalDangerousAssociationCheckService
    {
        public ScanAssociationByDeviceModel ScanAssociationByDevice(int batchSize);

        public void PreprocessDangerousAssociation(IEnumerable<PreprocessDangerousAssociationModel> data);
    }

    public class NormalDangerousAssociationCheckService : INormalDangerousAssociationCheckService
    {
        private readonly INormalDangerousAssociationCheckDataAccess _dataAccess;

        public NormalDangerousAssociationCheckService(INormalDangerousAssociationCheckDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public ScanAssociationByDeviceModel ScanAssociationByDevice(int batchSize)
        {
            return _dataAccess.ScanAssociationByDevice(batchSize);
        }

        public void PreprocessDangerousAssociation(IEnumerable<PreprocessDangerousAssociationModel> data)
        {
            if (data.IsNotNullOrEmpty())
            {
                _dataAccess.PreprocessDangerousAssociation(data);
            }
        }
    }
}
