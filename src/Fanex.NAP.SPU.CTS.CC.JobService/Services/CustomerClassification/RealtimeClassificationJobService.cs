namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Threading.Tasks;

    public interface IRealtimeClassificationJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize);
    }

    public class RealtimeClassificationJobService : IRealtimeClassificationJobService
    {
        private readonly string serviceName;
        private readonly IRealtimeClassificationService realtimeServices;

        public RealtimeClassificationJobService(IRealtimeClassificationService realtimeServices)
        {
            this.realtimeServices = realtimeServices;
            this.serviceName = "RealtimeClassification";
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize)
        {
            try
            {
                if (batchExternalSize > 0 && batchInternalSize > 0)
                {
                    this.realtimeServices.Start()
                        .GetNewPlaceBetCustomers()
                        .GetCustomerCategory(batchInternalSize);

                    Parallel.Invoke(() =>
                    {
                        this.realtimeServices.PreprocessCustomerClassRealtime(batchInternalSize);
                    },
                    () =>
                    {
                        this.realtimeServices.InsertPADailyScanQueue(batchInternalSize);
                    });

                    this.realtimeServices.CompleteCustomerRealtime();
                }
                else
                {
                    Utilities.LogSentryError(serviceName, "InValid Batch size", (batchExternalSize, batchInternalSize), new NullReferenceException());
                }
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
