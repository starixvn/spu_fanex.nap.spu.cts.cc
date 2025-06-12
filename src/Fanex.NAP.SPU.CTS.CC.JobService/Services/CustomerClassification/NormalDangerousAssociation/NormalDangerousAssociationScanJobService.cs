namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.NormalDangerousAssociation
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System;
    using System.Threading.Tasks;
    using Sentry;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RescanCustomerClassification;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;
    using System.Linq;

    public interface INormalDangerousAssociationScanJobService
    {
        Task<bool> Execute(int internalBatchSize, int externalBatchSize);
    }

    public class NormalDangerousAssociationScanJobService : INormalDangerousAssociationScanJobService
    {
        private const string _serviceName = "NormalDangerousAssociationScan";
        private readonly INormalDangerousAssociationScanService _service;
        private readonly ISystemParameterServices _parameterServices;
        private readonly IRescanCustClassificationService _rescanService;
        private readonly ISentryClient _sentryClient;

        public NormalDangerousAssociationScanJobService(INormalDangerousAssociationScanService service, ISentryClient sentryClient, ISystemParameterServices parameterServices, IRescanCustClassificationService rescanService)
        {
            _service = service;
            _sentryClient = sentryClient;
            _parameterServices = parameterServices;
            _rescanService = rescanService;
        }

        public Task<bool> Execute(int internalBatchSize, int externalBatchSize)
        {
            try
            {
                if (externalBatchSize <= 0)
                {
                    externalBatchSize = 5000;
                }

                if (internalBatchSize <= 0)
                {
                    internalBatchSize = 2000;
                }

                JobRun(internalBatchSize, externalBatchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(_serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int internalBatchSize, int externalBatchSize)
        {
            var scanData = _service.ScanDangerousAssociation(externalBatchSize);

            var dataChunk = scanData.CustIds.Chunk(internalBatchSize);
            foreach (var chunk in dataChunk)
            {
                var custIds = _service.CheckDangerousAssociationPerformance(chunk);

                if (custIds.IsNotNullOrEmpty())
                {
                    _service.InsertDangerousAssociationDataSource(custIds);
                    _rescanService.PreNormalClassification(custIds, FunctionId.NormalDangerousTagging, RescanNormalType.Auto_DangerousAssociation);
                }
            }

            _service.CompleteDangerousAssociation(scanData.LastScannedID);
            _parameterServices.UpdateTimingExecution(scanData.LastScannedID.ToString(), CTS_SystemParameters.NormalDangerousAssociationScan_LastScannedID);
        }
    }
}
