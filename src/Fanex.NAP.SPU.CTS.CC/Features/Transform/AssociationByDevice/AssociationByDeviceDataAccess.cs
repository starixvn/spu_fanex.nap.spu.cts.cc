namespace Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice.Criteria;
    using Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAssociationByDeviceDataAccess
    {
        IEnumerable<RawAssociationModel> GetRawAssociation(int quantityPerBatch, int numOfBatch);

        void TransformAssociationByDevice(string associationJson);
    }

    public class AssociationByDeviceDataAccess : IAssociationByDeviceDataAccess
    {
        readonly IDynamicRepository repository;

        public AssociationByDeviceDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RawAssociationModel> GetRawAssociation(int quantityPerBatch, int numOfBatch)
        {
            var criteria = new GetRawAssociationListCriteria
            {
                NumberOfAssociation = quantityPerBatch,
                NumberOfBatch = numOfBatch
            };

            return repository.Fetch<RawAssociationModel>(criteria);
        }

        public void TransformAssociationByDevice(string associationJson)
        {
            var criteria = new TransformAssociationByDeviceCriteria
            {
                AssociationList = associationJson
            };

            repository.Execute(criteria);
        }
    }
}
