namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalAccountInsert;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.RescanCustomerClassification;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface INormalTaggingClassificationDataAccess
    {
        public GetNormalTaggingScanningModel GetNormalTaggingScanning();

        public IEnumerable<long> CheckChangesNormalTaggingScanning(string taggedCustJsonString);

        public void PreprocessCustomerClassNormalTagging(string custIdList);
    }

    public class NormalTaggingClassificationDataAccess : INormalTaggingClassificationDataAccess
    {
        private readonly IDynamicRepository _repository;

        public NormalTaggingClassificationDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public GetNormalTaggingScanningModel GetNormalTaggingScanning()
        {
            var criteria = new NormalTaggingScanningGetCriteria();

            return new GetNormalTaggingScanningModel
            {
                CustIDs = _repository.Fetch<long>(criteria),
                LastCategoryID = criteria.LastCategoryID,
                LastCustID = criteria.LastCustID
            };
        }

        public IEnumerable<long> CheckChangesNormalTaggingScanning(string taggedCustJsonString)
        {
            var criteria = new NormalTaggingScanningCheckChangesCriteria
            {
                TaggingCust = taggedCustJsonString
            };

            return _repository.Fetch<long>(criteria);
        }

        public void PreprocessCustomerClassNormalTagging(string custIdList)
        {
            var criteria = new NormalClassificationRescanCriteria
            {
                ListCustId = custIdList,
                FunctionId = (int)FunctionId.NormalTaggingJob,
                RescanTypeId = (int)RescanNormalType.Auto_NormalTaggingScan
            };

            _repository.Execute(criteria);
        }
    }
}