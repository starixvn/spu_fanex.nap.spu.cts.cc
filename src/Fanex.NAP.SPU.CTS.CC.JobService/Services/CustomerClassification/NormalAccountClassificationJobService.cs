namespace Fanex.NAP.SPU.CTS.CC.JobService.Services.CustomerClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountClassification.Model;
    using Fanex.NAP.SPU.CTS.Shares;
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
        private readonly INormalAccountClassificationService normalAccountService;

        public NormalAccountClassificationJobService(INormalAccountClassificationService normalAccountService)
        {
            this.normalAccountService = normalAccountService;
            this.serviceName = "NormalAccountClassificationService";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        public Task<bool> Execute(int batchInternalSize)
        {
            try
            {
                if (batchInternalSize <= 0)
                {
                    batchInternalSize = 5000;
                }

                JobRun(batchInternalSize);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError(serviceName, ex.Message, string.Empty, ex);

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void JobRun(int batchInternalSize)
        {
            var normalPoolCustomerModel = normalAccountService.GetNormalPoolCustomers();

            var isExistCusts = normalPoolCustomerModel != null && normalPoolCustomerModel.NormalPoolCustomers.Any();

            if (isExistCusts)
            {
                ClassifyNormalAccountWithBatch(normalPoolCustomerModel, batchInternalSize);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
        private void ClassifyNormalAccountWithBatch(GetNormalPoolCustomerModel normalPoolCustomer, int batchInternalSize)
        {
            var customerList = normalPoolCustomer.NormalPoolCustomers;

            if (customerList != null && customerList.Any())
            {
                var custChunks = customerList.Chunk(batchInternalSize);

                custChunks
                    .AsParallel()
                    .WithDegreeOfParallelism(4)
                    .ForAll(custBatch => ClassifyNormalAccount(custBatch, normalPoolCustomer.ScannedMaxId));
            }
        }

        private void ClassifyNormalAccount(IEnumerable<GetNormalPoolCustomerEntity> normalPoolCustomers, long scannedMaxID)
        {
            var custInfos = normalPoolCustomers?.Select(x => new GetCurrentCCParamModel { CustID = x.CustId, ScanSpecialLicSubType = x.ScanSpecialLicSubType });

            var customerCategories = normalAccountService.GetCurrentCustomerCategory(custInfos);

            var customerInfos = normalPoolCustomers.Join(customerCategories, custMain => custMain.CustId,
                                                  custCTS => custCTS.CustID,
                                                  (custMain, custCTS) => new GetCurrentCustomerCategoryModel
                                                  {
                                                      CustId = custMain.CustId,
                                                      CategoryId = custCTS.CategoryID,
                                                      IsNewCreated = custCTS.IsNewCreated,
                                                      IsProbationLastDay = custCTS.IsProbationLastDay,
                                                      IsSpecialLicSubCC = custCTS.IsSpecialLicSubCC,
                                                      IsRealtimeOnly = custMain.IsRealtimeOnly,
                                                      ScanTaggingType = custMain.ScanTaggingType,
                                                      ScanSpecialLicSubType = custMain.ScanSpecialLicSubType
                                                  });

            var custIds = string.Join(",", normalPoolCustomers.Select(x => x.CustId).Distinct());

            normalAccountService.ClassifyNormalPoolCustomers(customerInfos.ToXmlString())
                                .ClearNormalPoolCustomers(custIds, scannedMaxID);
        }
    }
}
