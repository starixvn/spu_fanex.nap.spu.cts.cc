namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountClassification;
    using Fanex.NAP.SPU.CTS.Shares;
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
                var isContinue = true;

                if (batchInternalSize <= 0)
                    batchInternalSize = 5000;

                do
                {
                    isContinue = JobRun(batchInternalSize);

                } while (isContinue);
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
                var customerChunks = normalPoolCustomerModel.NormalPoolCustomers.Chunk(batchInternalSize);

                var actions = new List<Action>();

                foreach (var custBatch in customerChunks)
                {
                    actions.Add(() => ClassifyNormalAccount(custBatch, normalPoolCustomerModel.ScannedMaxId));
                }

                Parallel.Invoke(actions.ToArray());
            }

            return isExistCusts;
        }

        private void ClassifyNormalAccount(IEnumerable<GetNormalPoolCustomerEntity> normalPoolCustomers, long scannedMaxID)
        {
            Utilities.LogSentryInfo(serviceName, "ClassifyNormalAccount normalPoolCustomers ", normalPoolCustomers, true, sentryClient);

            var custIDList = string.Join(",", normalPoolCustomers.Select(c => c.CustId).Distinct());

            var customerCategories = normalAccountService.GetCurrentCustomerCategory(custIDList);

            var customerInfos = normalPoolCustomers.Join(customerCategories, custMain => custMain.CustId,
                                                  custCTS => custCTS.CustID,
                                                  (custMain, custCTS) => new GetCurrentCustomerCategoryModel
                                                  {
                                                      CustId = custMain.CustId,
                                                      CategoryId = custCTS.CategoryID,
                                                      IsNewCreated = custCTS.IsNewCreated,
                                                      IsCheckBetCount = custMain.IsCheckBetCount
                                                  });

            normalAccountService.ClassifyNormalPoolCustomers(customerInfos.ToXmlString())
                .ClearNormalPoolCustomers(custIDList, scannedMaxID);
        }

    }
}
