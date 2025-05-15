namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Fanex.Data.Repository;    
    using System.Collections.Generic;
<<<<<<< .mine

=======
    using Data.Repository;
>>>>>>> .theirs

    public interface ITWClassificationDataAccess
    {
<<<<<<< .mine
        IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize);

=======
        IEnumerable<TWCustomerResponse> GetSpecialLicSubCCs();
        void UpdateCustomerResponseStatus(long id, int processStatus, int responseHttpCode);
>>>>>>> .theirs
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
        private readonly IDynamicRepository repository;

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
            {
                IDs = ids
            };

            repository.Execute(criteria);
        }
    }
}
