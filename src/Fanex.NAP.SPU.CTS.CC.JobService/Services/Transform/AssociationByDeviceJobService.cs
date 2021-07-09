namespace Fanex.NAP.SPU.CTS.JobService.Services
{
    using Fanex.NAP.SPU.CTS.Features.Transform.AssociationByDevice;
    using Sentry;
    using System;
    using System.Threading.Tasks;

    public interface IAssociationByDeviceJobService
    {
        Task<bool> TransformAssociationByDevice(int batchSize, int numOfBatch);
    }

    public class AssociationByDeviceJobService : IAssociationByDeviceJobService
    {
        private readonly IAssociationByDeviceService associationByDeviceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public AssociationByDeviceJobService(ISentryClient sentryClient, IAssociationByDeviceService associationByDeviceService, IAppSettings appSettings)
        {
            this.sentryClient = sentryClient;
            this.associationByDeviceService = associationByDeviceService;
            this.appSettings = appSettings;
        }

        public Task<bool> TransformAssociationByDevice(int batchSize, int numOfBatch)
        {
            try
            {
                associationByDeviceService.TransformAssociationByDevice(batchSize, numOfBatch, appSettings.TransformAssByDeviceIsWriteLog);
                sentryClient.CaptureMessage("Done TransformAssociationByDevice");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
                sentryClient.CaptureMessage("Done TransformAssociationByDevice with error");
                return Task.FromResult(false);
            }
        }
    }
}
