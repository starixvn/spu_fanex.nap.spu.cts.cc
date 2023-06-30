namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport.Renovate
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.NormalAccountClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface INormalAccountClassificationBySportJobService
    {
        Task<bool> Execute(int batchInternalSize);
    }

    public class NormalAccountClassificationBySportJobService : INormalAccountClassificationBySportJobService
    {
        private readonly string serviceName;
        private readonly ISentryClient sentryClient;
        private readonly INormalAccountClassificationBySportService normalClassifyService;

        public NormalAccountClassificationBySportJobService(ISentryClient sentryClient, INormalAccountClassificationBySportService normalClassifyService)
        {
            this.sentryClient = sentryClient;
            this.normalClassifyService = normalClassifyService;
            this.serviceName = "NormalAccountClassificationBySportService";
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
            var normalPoolCustomerModel = normalClassifyService.GetNormalPoolCustomers();

            var isExistCusts = normalPoolCustomerModel != null && normalPoolCustomerModel.NormalPoolCustomers.Any();

            if (isExistCusts)
            {
                var customerChunks = normalPoolCustomerModel.NormalPoolCustomers.Chunk(batchInternalSize);

                var actions = new List<Action>();

                foreach (var custBatch in customerChunks)
                {
                    actions.Add(() => ClassifyNormalAccountBySport(custBatch, normalPoolCustomerModel.ScannedMaxId));
                }

                Parallel.Invoke(actions.ToArray());
            }

            return isExistCusts;
        }

        private void ClassifyNormalAccountBySport(IEnumerable<GetNormalPoolCustomerBySportEntity> normalPoolCustomers, long scannedMaxID)
        {
            var custIDList = string.Join(",", normalPoolCustomers.Select(c => c.CustId).Distinct());

            var customerCategories = normalClassifyService.GetCurrentCustomerCategory(custIDList);

            var customerInfos = normalPoolCustomers.Join(customerCategories, custMain => custMain.CustId,
                                                  custCTS => custCTS.CustID,
                                                  (custMain, custCTS) => new GetCurrentCustomerCategoryBySportModel
                                                  {
                                                      CustId = custMain.CustId,
                                                      CategoryId = custCTS.CategoryID,
                                                      IsNewCreated = custCTS.IsNewCreated,
                                                      IsProbationLastDay = custCTS.IsProbationLastDay,
                                                      IsCheckBetCount = custMain.IsCheckBetCount
                                                  });

            var customerInfoClears = normalPoolCustomers.Select(x => new CustInfoNormalPoolClearBySportModel { CustId = x.CustId, SportGroup = x.SportGroup }).Distinct();

            normalClassifyService.ClassifyNormalPoolCustomers(customerInfos.ToXmlString())
                                .ClearNormalPoolCustomers(customerInfoClears.ToXmlString(), scannedMaxID);
        }
    }
}
