namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public interface INormalDangerousAssociationCheckDataAccess
    {
        public void PreprocessDangerousAssociation(IEnumerable<PreprocessDangerousAssociationModel> data);

        public ScanAssociationByDeviceModel ScanAssociationByDevice(int batchSize);
    }

    public class NormalDangerousAssociationCheckDataAccess : INormalDangerousAssociationCheckDataAccess
    {
        private readonly IDynamicRepository _repository;

        public NormalDangerousAssociationCheckDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public void PreprocessDangerousAssociation(IEnumerable<PreprocessDangerousAssociationModel> data)
        {
            var criteria = new PreprocessDangerousAssociationCriteria
            {
                CustInfoJson = JsonConvert.SerializeObject(data)
            };

            _repository.Execute(criteria);
        }

        public ScanAssociationByDeviceModel ScanAssociationByDevice(int batchSize)
        {
            var criteria = new ScanAssociationByDeviceCriteria
            {
                BatchSize = batchSize
            };

            return new ScanAssociationByDeviceModel
            {
                Data = _repository.Fetch<ScanAssociationByDeviceEntity>(criteria),
                LastCTSAssDevID = criteria.LastCTSAssDevID
            };
        }
    }
}
