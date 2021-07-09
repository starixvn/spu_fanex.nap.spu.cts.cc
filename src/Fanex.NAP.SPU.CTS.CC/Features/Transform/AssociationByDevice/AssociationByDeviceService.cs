namespace Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice
{
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAssociationByDeviceService
    {
        void TransformAssociationByDevice(int numOfAssociation, int numOfBatch);
    }

    public class AssociationByDeviceService : IAssociationByDeviceService
    {
        private readonly IAssociationByDeviceDataAccess dataAccess;
        private readonly ISentryClient sentryClient;

        public AssociationByDeviceService(IAssociationByDeviceDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public void TransformAssociationByDevice(int numOfAssociation, int numOfBatch)
        {
            var listAssociation = dataAccess.GetRawAssociation(numOfAssociation, numOfBatch);

            if (listAssociation.Any())
            {
                var groupTrans = listAssociation.GroupBy(x => x.BatchId);

                Parallel.ForEach(groupTrans, trans =>
                {
                    var associationJson = JsonConvert.SerializeObject(trans.ToList());
                    dataAccess.TransformAssociationByDevice(associationJson);
                });
            }
        }
    }
}
