using System.Collections.Generic;

namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NormalTaggingClassification
{
    public interface INormalTaggingClassificationDataAccess
    {
        public IEnumerable<long> GetTaggingCustIds(int batchSize);

        public void PreprocessCustomerClassNormalTagging(string custIdList);

    }

    public class NormalTaggingClassificationDataAccess : INormalTaggingClassificationDataAccess
    {
        public IEnumerable<long> GetTaggingCustIds(int batchSize)
        {
            throw new System.NotImplementedException();
        }

        public void PreprocessCustomerClassNormalTagging(string custIdList)
        {
            throw new System.NotImplementedException();
        }
    }
}