namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.DailyProblemClassificationBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Model;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemWLClassificationBySportService
    {
        IProblemWLClassificationBySportService InsertPAWLClassificationBySport(IEnumerable<ProblemAccountLosingBySportEntity> paWLClassification);

        bool PushCCBySportToExternal(string serviceName, int batchExternalSize);
    }

    public class ProblemWLClassificationBySportService : IProblemWLClassificationBySportService
    {
        private readonly IProblemWLClassificationBySportDataAccess _dataAccess;
        private readonly ICustClassToMainDBService _custClassToMainDB;

        private IEnumerable<InsertPABySportResultModel> PAWLs;

        public ProblemWLClassificationBySportService(
            IProblemWLClassificationBySportDataAccess dataAccess,
            ICustClassToMainDBService custClassToMainDB)
        {
            _dataAccess = dataAccess;
            _custClassToMainDB = custClassToMainDB;

            PAWLs = new List<InsertPABySportResultModel>();
        }

        public IProblemWLClassificationBySportService InsertPAWLClassificationBySport(IEnumerable<ProblemAccountLosingBySportEntity> paWLClassification)
        {
            if (paWLClassification.IsNotNullOrEmpty())
            {
                var paJson = JsonConvert.SerializeObject(paWLClassification);

                PAWLs = _dataAccess.InsertPAWLPerformanceBySport(paJson);
            }

            return this;
        }

        public bool PushCCBySportToExternal(string serviceName, int batchExternalSize)
        {
            if (PAWLs.IsNotNullOrEmpty())
            {
                var custs = PAWLs.Distinct();

                return _custClassToMainDB.PushCCBySportToMainDB(custs, serviceName, batchExternalSize);
            }

            return true;
        }
    }
}
