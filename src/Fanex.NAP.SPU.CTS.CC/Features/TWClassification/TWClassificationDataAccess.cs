namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Fanex.Data.Repository;
    using Criteria;
    using System.Collections.Generic;

    public interface ITWClassificationDataAccess
    {
        IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize);

        IEnumerable<TWCustomerResponse> GetSpecialLicSubCCs();

        void UpdateCustomerResponseStatus(long id, int processStatus, int responseHttpCode);

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

        public IEnumerable<TWCustomerResponse> GetSpecialLicSubCCs()
        {
            var criteria = new GetSpecialLicSubCCCriteria();

            return repository.Fetch<TWCustomerResponse>(criteria);
        }

        public void UpdateCustomerResponseStatus(long id, int processStatus, int responseHttpCode)
        {
            var criteria = new CompleteSpecialLicSubCCCriteria
            {
                ID = id,
                ProcessStatus = processStatus,
                ResponseHttpCode = responseHttpCode
            };

            repository.Execute(criteria);
        }
    }
}
