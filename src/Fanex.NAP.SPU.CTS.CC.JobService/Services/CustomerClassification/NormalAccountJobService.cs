namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.AntiFraudCenter;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.BaseCustomerClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalAccountJobService
    {
        Task<bool> Execute(int batchExternalSize, int batchInternalSize, bool isLogData);
    }

    public class NormalAccountJobService : INormalAccountJobService
    {
        private readonly string _serviceName;
        private readonly ISentryClient _sentryClient;
        private readonly IAppSettings _appSettings;
        private readonly INormalAccountService _normalAccountService;
        private readonly ICustClassToMainDBService _custClassToMainDBService;
        private readonly IAntiFraudCenterServices _antiFraudCenterServices;
        private readonly INormalDangerousAssociationCheckService _normalDangerousAssociationCheckService;

        public NormalAccountJobService(ISentryClient sentryClient,
            IAppSettings appSettings,
            INormalAccountService normalAccountService,
            ICustClassToMainDBService pushDataToExternalService,
            IAntiFraudCenterServices antiFraudCenterServices,
            INormalDangerousAssociationCheckService normalDangerousAssociationCheckService)
        {
            _sentryClient = sentryClient;
            _normalAccountService = normalAccountService;
            _custClassToMainDBService = pushDataToExternalService;
            _appSettings = appSettings;
            _antiFraudCenterServices = antiFraudCenterServices;
            _serviceName = "NormalAccountInsert";
            _normalDangerousAssociationCheckService = normalDangerousAssociationCheckService;
        }

        public Task<bool> Execute(int batchExternalSize, int batchInternalSize, bool isLogData)
        {
            try
            {
                var isContinue = true;

                if (batchInternalSize <= 0)
                    batchInternalSize = 5000;

                do
                {
                    isContinue = JobRun(batchExternalSize, batchInternalSize, isLogData);

                } while (isContinue);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(_serviceName, ex.Message, string.Empty, ex, _sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun(int batchExternalSize, int batchInternalSize, bool isLogData)
        {
            var normalAccountModel = _normalAccountService.GetNormalAccounts(isLogData);

            if (normalAccountModel.IsContinute)
            {
                var failedThreads = new ConcurrentBag<bool>();

                var custChunks = normalAccountModel.NormalAccount.Chunk(batchInternalSize);

                custChunks
                    .AsParallel()
                    .WithDegreeOfParallelism(5)
                    .ForAll(custBatch => InsertNormalAccount(custBatch, batchExternalSize, failedThreads));

                _normalAccountService.CompleteNormalAccountInsert(normalAccountModel.ScannedMaxId, failedThreads.IsEmpty);
            }

            return normalAccountModel.IsContinute;
        }

        private void InsertNormalAccount(IEnumerable<GetNormalAccountEntity> custs, int batchExternalSize, ConcurrentBag<bool> failedThreads)
        {
            var finalNormalAcc = _normalAccountService.MergeWithTaggedNormalTaggingFlow(custs);

            var result = _normalAccountService.InsertNormalAccount(finalNormalAcc, _serviceName, _appSettings.NormalAccountIsWriteLog);

            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                var custIDs = result.CustInfos.Select(x => x.CustID).Distinct();

                Parallel.Invoke((() =>
                {
                    var isPushCCSuccess = _custClassToMainDBService.PushExternalPackageByCustId(custIDs, new PushDataToExternalEntity
                    {
                        RoleGroup = RoleGroupType.Member,
                        ExternalBatchSize = batchExternalSize,
                        IsUpdateDangerToMainDB = true,
                        IsWriteLog = _appSettings.NormalAccountIsWriteLog,
                        ServiceName = _serviceName
                    });

                    if (!isPushCCSuccess)
                    {
                        failedThreads.Add(false);
                    }
                }),
                () =>
                {
                    PreprocessDangerousAssociation(result.CustInfos);
                },
                () =>
                {
                    _antiFraudCenterServices.DeleteAFCAccounts(custIDs, FromFlow.CTSServices, _appSettings.IsSyncAFCData, _appSettings.AFCDeleteAccountURL);
                });
            }
        }

        private void PreprocessDangerousAssociation(IEnumerable<CustInfoClassificationEntity> custInfos)
        {
            var preprocessSRPChange = custInfos
                                            .Where(x => x.IsScanDgrAssociation)
                                            .Select(x => new PreprocessDangerousAssociationModel
                                            {
                                                CTSCustID = x.CTSCustID,
                                                DCSDeviceID = null
                                            });

            _normalDangerousAssociationCheckService.PreprocessDangerousAssociation(preprocessSRPChange);
        }
    }
}
