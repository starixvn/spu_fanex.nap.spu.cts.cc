namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.NormalDangerousAssociation
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.SystemParameter;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalDangerousAssociationCheckJobService
    {
        Task<bool> Execute(int batchSize);
    }

    public class NormalDangerousAssociationCheckJobService : INormalDangerousAssociationCheckJobService
    {
        private const string serviceName = "NormalDangerousAssociationCheck";
        private readonly INormalDangerousAssociationCheckService _service;
        private readonly ISystemParameterServices _parameterServices;
        private readonly ISentryClient _sentryClient;

        public NormalDangerousAssociationCheckJobService(INormalDangerousAssociationCheckService service, ISystemParameterServices parameterServices, ISentryClient sentryClient)
        {
            _service = service;
            _parameterServices = parameterServices;
            _sentryClient = sentryClient;
        }

        public Task<bool> Execute(int batchSize)
        {
            try
            {
                if (batchSize <= 0)
                {
                    batchSize = 5000;
                }

                JobRun(batchSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchSize)
        {
            var data = _service.ScanAssociationByDevice(batchSize);

            PreprocessDangerousAssociation(data.Data);

            _parameterServices.UpdateTimingExecution(data.LastCTSAssDevID.ToString(), CTS_SystemParameters.NormalDangerousAssociationCheck_LastCTSAssDevID);
        }

        private void PreprocessDangerousAssociation(IEnumerable<ScanAssociationByDeviceEntity> custInfos)
        {
            var preprocessSRPChange = custInfos
                                        .Select(x => new PreprocessDangerousAssociationModel
                                        {
                                            CTSCustID = x.CTSCustID,
                                            DCSDeviceID = x.DCSDeviceID,
                                        });

            _service.PreprocessDangerousAssociation(preprocessSRPChange);
        }
    }
}
