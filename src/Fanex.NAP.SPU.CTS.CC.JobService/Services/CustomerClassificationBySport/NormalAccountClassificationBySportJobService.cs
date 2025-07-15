namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.NormalAccountClassificationBySport;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    
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
        private readonly INormalAccountClassificationBySportService normalClassifyService;

        public NormalAccountClassificationBySportJobService(INormalAccountClassificationBySportService normalClassifyService)
        {
            this.normalClassifyService = normalClassifyService;
            this.serviceName = "NormalAccountClassificationBySport";
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
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex);

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

                customerChunks.AsParallel().WithDegreeOfParallelism(5).ForAll(custBatch => ClassifyNormalAccountBySport(custBatch, normalPoolCustomerModel.ScannedMaxId));
            }

            return isExistCusts;
        }

        private void ClassifyNormalAccountBySport(IEnumerable<GetNormalPoolCustomerBySportEntity> normalPoolCustomers, long scannedMaxID)
        {
            var custSportList = JsonConvert.SerializeObject(normalPoolCustomers.Select(x => new CustSportModel
            {
                CustId = x.CustId,
                SportGroup = x.SportGroup
            }).Distinct());

            var customerInfoClears = normalPoolCustomers.Select(x => new CustInfoNormalPoolClearBySportModel { CustId = x.CustId, SportGroup = x.SportGroup }).Distinct();

            var customerCategories = normalClassifyService.GetCurrentCustomerCategory(custSportList);

            var customerInfos = normalPoolCustomers.Join(customerCategories, custMain => new { CustID = custMain.CustId, custMain.SportGroup },
                                                  custCTS => new { custCTS.CustID, custCTS.SportGroup },
                                                  (custMain, custCTS) => new GetCurrentCustomerCategoryBySportModel
                                                  {
                                                      CustID = custMain.CustId,
                                                      SportGroup = custMain.SportGroup,
                                                      CategoryID = custCTS.CategoryID,
                                                      IsProbationLastDay = custCTS.IsProbationLastDay,
                                                      IsCheckBetCount = custMain.IsCheckBetCount
                                                  });

            normalClassifyService.ClassifyNormalPoolCustomers(customerInfos.ToXmlString())
                                .ClearNormalPoolCustomers(customerInfoClears.ToXmlString(), scannedMaxID);
        }
    }
}
