namespace Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice
{
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAssociationByDeviceService
    {
        void TransformAssociationByDevice(int numOfAssociation, int numOfBatch, bool isWriteLog);
    }

    public class AssociationByDeviceService : IAssociationByDeviceService
    {
        private readonly IAssociationByDeviceDataAccess dataAccess;
        private readonly ISentryClient sentryClient;
        private readonly string serviceName = "AssociationByDeviceService";

        public AssociationByDeviceService(IAssociationByDeviceDataAccess dataAccess, ISentryClient sentryClient)
        {
            this.dataAccess = dataAccess;
            this.sentryClient = sentryClient;
        }

        public void TransformAssociationByDevice(int numOfAssociation, int numOfBatch, bool isWriteLog)
        {
            var listAssociation = dataAccess.GetRawAssociation(numOfAssociation, numOfBatch);
            Utilities.LogSentryInfo(serviceName, "GetRawAssociation", listAssociation, isWriteLog, sentryClient);

            if (listAssociation.Any())
            {
                var groupTrans = listAssociation.GroupBy(x => x.BatchId);

                Parallel.ForEach(groupTrans, trans =>
                {
                    var associationJson = JsonConvert.SerializeObject(trans.ToList());
                    Utilities.LogSentryInfo(serviceName, "TransformAssociationByDevice", trans, isWriteLog, sentryClient);
                    dataAccess.TransformAssociationByDevice(associationJson);
                });
            }
        }
    }
}
