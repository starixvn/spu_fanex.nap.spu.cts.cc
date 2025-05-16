namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Data.Repository;
    using Criteria;
    using System.Collections.Generic;

    public interface ITWClassificationDataAccess
    {
        IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize);

        IEnumerable<SpecialLicSubCCResponseGetEntity> GetSpecialLicSubCCResponse(int batchSize);

        void CompleteSpecialLicSubCCResponse(string responseInfo);

        void PreprocessSpecialLicSubCCScan(string custIdList);

        void CompleteSpecialLicSubCCScan(string ids);
    }

    public class TWClassificationDataAccess : ITWClassificationDataAccess
    {        
        private readonly IDynamicRepository repository;

        public TWClassificationDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize)
        {
            var criteria = new SpecialLicSubCCScanGetCriteria() 
            {
                BatchSize = batchSize 
            };

            return repository.Fetch<SpecialLicSubCCScanGetEntity>(criteria);        
        }

        public void PreprocessSpecialLicSubCCScan(string custIdList)
        {
            var criteria = new LicSubTaggingPreprocessCriteria()
            {
                ListCustId = custIdList
            };

            repository.Execute(criteria);
        }
        
        public void CompleteSpecialLicSubCCScan(string ids)
        {
            var criteria = new SpecialLicSubCCScanCompleteCriteria()             
            {
                IDs = ids
            };

            repository.Execute(criteria);
        }

        public IEnumerable<SpecialLicSubCCResponseGetEntity> GetSpecialLicSubCCResponse(int batchSize)
        {
            var criteria = new SpecialLicSubCCResponseGetCriteria
            {
                BatchSize = batchSize
            };

            return repository.Fetch<SpecialLicSubCCResponseGetEntity>(criteria);
        }

        public void CompleteSpecialLicSubCCResponse(string responseInfo)
        {
            var criteria = new SpecialLicSubCCResponseCompleteCriteria
            {
                ResponseInfo = responseInfo
            };

            repository.Execute(criteria);
        }
    }
}
