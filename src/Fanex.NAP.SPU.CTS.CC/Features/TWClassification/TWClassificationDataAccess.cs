namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Criteria;
    using Entities;
    using System.Collections.Generic;
    using Data.Repository;

    public interface ITWClassificationDataAccess
    {
        IEnumerable<TWCustomerResponse> GetSpecialLicSubCCs();
        void UpdateCustomerResponseStatus(long id, int processStatus, int responseHttpCode);
    }

    public class TWClassificationDataAccess : ITWClassificationDataAccess
    {
        private readonly IDynamicRepository repository;

        public TWClassificationDataAccess()
        {
            repository = new DynamicRepository();
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
