namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification;
    using System.Collections.Generic;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalTaggingClassificationDataAccess
    {
        public IEnumerable<long> GetTaggingCustIds(int batchSize);

        public void PreprocessCustomerClassNormalTagging(string custIdList);

    }

    public class NormalTaggingClassificationDataAccess : INormalTaggingClassificationDataAccess
    {
        private readonly IDynamicRepository _repository;

        public NormalTaggingClassificationDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<long> GetTaggingCustIds(int batchSize)
        {
            var criteria = new NormalTaggingScannningGetCriteria
            {
                NumberOfRecord = batchSize
            };

            return _repository.Fetch<long>(criteria);
        }

        public void PreprocessCustomerClassNormalTagging(string custIdList)
        {
            var criteria = new NormalClassificationRescanCriteria
            {
                ListCustId = custIdList,
                FunctionId = (int)FunctionId.NormalTaggingJob,
                RescanTypeId = (int)RescanNormalType.Auto_AfterNormalScan
            };

            _repository.Execute(criteria);
        }
    }
}