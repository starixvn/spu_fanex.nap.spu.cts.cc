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
                var customerNotWithTaggingList = normalPoolCustomerModel.NormalPoolCustomers.Where(x => !x.IsScanOnlyTagging);

                var customerWithTaggingList = normalPoolCustomerModel.NormalPoolCustomers.Where(x => x.IsScanOnlyTagging);

                ClassifyNormalAccountWithBatch(customerNotWithTaggingList, normalPoolCustomerModel.ScannedMaxId, batchInternalSize);

                ClassifyNormalAccountWithBatch(customerWithTaggingList, normalPoolCustomerModel.ScannedMaxId, batchInternalSize);
            }

            return isExistCusts;
        }

        private void ClassifyNormalAccountWithBatch(IEnumerable<GetNormalPoolCustomerEntity> customerList, long sacnnedMaxId, int batchInternalSize)
        {
            var actions = new List<Action>();

            var idx = 4;

            foreach (var custBatch in customerList.Chunk(batchInternalSize))
            {
                actions.Add(() => ClassifyNormalAccount(custBatch, sacnnedMaxId));

                if (actions.Count == idx)
                {
                    Parallel.Invoke(actions.ToArray());
                }
            }

            if (actions.Any())
            {
                Parallel.Invoke(actions.ToArray());
            }
        }

        private void ClassifyNormalAccount(IEnumerable<GetNormalPoolCustomerEntity> normalPoolCustomers, long scannedMaxID)
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
                                                      IsScanOnlyTagging = custMain.IsScanOnlyTagging
                                                  });

            normalAccountService.ClassifyNormalPoolCustomers(customerInfos.ToXmlString())
                                .ClearNormalPoolCustomers(custIDList, scannedMaxID);
        }

    }
}
