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
        public GetNormalTaggingScanningModel GetNormalTaggingScanning(int batchSize);

        public IEnumerable<long> CheckChangesNormalTaggingScanning(IEnumerable<CheckChangesNormalTaggingScanningModel> taggedCustEntities);

        public void PreprocessCustomerClassNormalTagging(string custIdList);
    }

    public class NormalTaggingClassificationDataAccess : INormalTaggingClassificationDataAccess
    {
        private readonly IDynamicRepository _repository;

        public NormalTaggingClassificationDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public GetNormalTaggingScanningModel GetNormalTaggingScanning(int batchSize)
        {
            var criteria = new NormalTaggingScannningGetCriteria
            {
                NumberOfRecord = batchSize
            };

            return new GetNormalTaggingScanningModel
            {
                CustIDs = _repository.Fetch<long>(criteria),
                LastCategoryID = criteria.LastCategoryIDfRecord,
                LastCustID = criteria.LastCustID
            };
        }

        public IEnumerable<long> CheckChangesNormalTaggingScanning(IEnumerable<CheckChangesNormalTaggingScanningModel> taggedCustEntities)
        {
            var jsonString = taggedCustEntities != null ? JsonConvert.SerializeObject(taggedCustEntities) : null;
            
            if (jsonString != null)
            {
                var criteria = new NormalTaggingScanningCheckChangesCriteria
                {
                    TaggingCust = jsonString
                };

                return _repository.Fetch<long>(criteria);
            }

            return Enumerable.Empty<long>();
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