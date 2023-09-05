namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification;
    using Fanex.NAP.SPU.CTS.Shares;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalAccountClassificationJobService
    {
        Task<bool> Execute(int batchInternalSize);
    }

    public class NormalAccountClassificationJobService : INormalAccountClassificationJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountClassificationService normalAccountService;

        public NormalAccountClassificationJobService(ISentryClient sentryClient, INormalAccountClassificationService normalAccountService)
        {
            this.sentryClient = sentryClient;
            this.normalAccountService = normalAccountService;
            this.serviceName = "NormalAccountClassificationService";
        }

        public Task<bool> Execute(int batchInternalSize)
        {
            try
            {
                if (batchInternalSize <= 0)
                    batchInternalSize = 5000;

                JobRun(batchInternalSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex, sentryClient);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private bool JobRun(int batchInternalSize)
        {
            var normalPoolCustomerModel = normalAccountService.GetNormalPoolCustomers();

            var isExistCusts = normalPoolCustomerModel != null && normalPoolCustomerModel.NormalPoolCustomers.Any();

            if (isExistCusts)
            {
                // classifyCustomer
                ClassifyNormalAccountWithBatch(normalPoolCustomerModel, batchInternalSize, isScanOnlyTagging: false);

                // taggingCustomer
                ClassifyNormalAccountWithBatch(normalPoolCustomerModel, batchInternalSize, isScanOnlyTagging: true);
            }

            return isExistCusts;
        }

        private void ClassifyNormalAccountWithBatch(GetNormalPoolCustomerModel normalPoolCustomer, int batchInternalSize, bool isScanOnlyTagging)
        {
            var customerList = isScanOnlyTagging ? normalPoolCustomer.NormalPoolCustomers.Where(x => x.ScanTaggingType == (int)NormalClasification_TaggingType.ExistOnly)
                                                 : normalPoolCustomer.NormalPoolCustomers.Where(x => x.ScanTaggingType != (int)NormalClasification_TaggingType.ExistOnly);

            if (customerList != null && customerList.Any())
            {
                var custChunks = customerList.Chunk(batchInternalSize);

                custChunks.AsParallel().WithDegreeOfParallelism(4).ForAll(custBatch => ClassifyNormalAccount(custBatch, normalPoolCustomer.ScannedMaxId, isScanOnlyTagging));
            }
        }

        private void ClassifyNormalAccount(IEnumerable<GetNormalPoolCustomerEntity> normalPoolCustomers, long scannedMaxID, bool isScanOnlyTagging)
        {
            var custIDList = string.Join(",", normalPoolCustomers.Select(c => c.CustId).Distinct());

            var customerCategories = normalAccountService.GetCurrentCustomerCategory(custIDList);

            var customerInfos = normalPoolCustomers.Join(customerCategories, custMain => custMain.CustId,
                                                  custCTS => custCTS.CustID,
                                                  (custMain, custCTS) => new GetCurrentCustomerCategoryModel
                                                  {
                                                      CustId = custMain.CustId,
                                                      CategoryId = custCTS.CategoryID,
                                                      IsNewCreated = custCTS.IsNewCreated,
                                                      IsProbationLastDay = custCTS.IsProbationLastDay,
                                                      IsCheckBetCount = custMain.IsCheckBetCount,
                                                      ScanTaggingType = custMain.ScanTaggingType
                                                  });

            normalAccountService.ClassifyNormalPoolCustomers(customerInfos.ToXmlString(), isScanOnlyTagging)
                                .ClearNormalPoolCustomers(custIDList, scannedMaxID);
        }

    }
}
