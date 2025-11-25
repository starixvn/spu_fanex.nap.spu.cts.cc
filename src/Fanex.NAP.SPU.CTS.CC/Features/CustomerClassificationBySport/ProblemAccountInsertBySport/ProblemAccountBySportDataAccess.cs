namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.Data.Repository; 
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Criteria;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IProblemAccountBySportDataAccess
    {
        IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(string custInfosJson, BySport_FunctionId functionId);
    }

    public class ProblemAccountBySportDataAccess : IProblemAccountBySportDataAccess
    {
        private readonly IDynamicRepository _repository;

        public ProblemAccountBySportDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(string custInfosJson, BySport_FunctionId functionId)
        {
            if (custInfosJson.IsNotNullOrEmpty())
            {
                var criteria = new ProblemAccountClassificationBySportGetCriteria
                {
                    CustJson = custInfosJson,
                    FunctionId = (int) functionId,
                };

                return _repository.Fetch<ProblemAccountLosingBySportEntity>(criteria);
            }

            return Enumerable.Empty<ProblemAccountLosingBySportEntity>();
        }      
    }
}
