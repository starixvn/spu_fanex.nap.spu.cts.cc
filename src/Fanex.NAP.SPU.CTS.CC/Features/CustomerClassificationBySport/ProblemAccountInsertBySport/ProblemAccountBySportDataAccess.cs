namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemAccountBySportDataAccess
    {
        IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(string custInfosJson);
    }

    public class ProblemAccountBySportDataAccess : IProblemAccountBySportDataAccess
    {
        private readonly IDynamicRepository _repository;

        public ProblemAccountBySportDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(string custInfosJson)
        {
            if (custInfosJson.IsNotNullOrEmpty())
            {
                var criteria = new ProblemAccountClassificationBySportGetCriteria
                {
                    CustJson = custInfosJson
                };

                return _repository.Fetch<ProblemAccountLosingBySportEntity>(criteria);
            }

            return Enumerable.Empty<ProblemAccountLosingBySportEntity>();
        }
    }
}
