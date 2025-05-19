namespace Fanex.NAP.SPU.CTS.CC.Features.TWClassification
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface ITWClassificationService
    {
        IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize);

        void PreprocessSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs);

        void CompleteSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs);

        IEnumerable<SpecialLicSubCCResponseGetEntity> GetSpecialLicSubCCResponse(int batchSize);

        void CompleteSpecialLicSubCCResponse(string responseInfo);

        void CompleteSpecialLicSubClassification(string custInfo);
    }

    public class TWClassificationService : ITWClassificationService
    {
        private readonly ITWClassificationDataAccess _twClassificationDataAccess;

        public TWClassificationService(ITWClassificationDataAccess twClassificationDataAccess)
        {
            this._twClassificationDataAccess = twClassificationDataAccess;
        }

        public IEnumerable<SpecialLicSubCCScanGetEntity> GetSpecialLicSubCCScan(int batchSize)
        {
            return _twClassificationDataAccess.GetSpecialLicSubCCScan(batchSize);
        }

        public void PreprocessSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs)
        {
            if (custLicSubs.Any())
            {
                var custIdList = string.Join(",", custLicSubs.Select(x => x.CustID).Distinct());

                _twClassificationDataAccess.PreprocessSpecialLicSubCCScan(custIdList);
            }
        }

        public void CompleteSpecialLicSubCCScan(IEnumerable<SpecialLicSubCCScanGetEntity> custLicSubs)
        {
            if (custLicSubs.Any())
            {
                var ids = string.Join(",", custLicSubs.Select(x => x.ID));

                _twClassificationDataAccess.CompleteSpecialLicSubCCScan(ids);
            }
        }

        public IEnumerable<SpecialLicSubCCResponseGetEntity> GetSpecialLicSubCCResponse(int batchSize)
        {
            return _twClassificationDataAccess.GetSpecialLicSubCCResponse(batchSize);
        }

        public void CompleteSpecialLicSubCCResponse(string responseInfo)
        {
            _twClassificationDataAccess.CompleteSpecialLicSubCCResponse(responseInfo);
        }


        public void CompleteSpecialLicSubClassification(string custInfo)
        {
            _twClassificationDataAccess.CompleteSpecialLicSubClassification(custInfo);
        }
    }
}
