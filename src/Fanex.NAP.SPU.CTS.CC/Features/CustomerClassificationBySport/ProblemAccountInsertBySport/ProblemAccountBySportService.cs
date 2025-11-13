namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProblemAccountInsertBySport.Entities;
    using Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB.Model;
    using Fanex.NAP.SPU.CTS.Shares;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProblemAccountBySportService
    {
        IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos);
    }

    public class ProblemAccountBySportService : IProblemAccountBySportService
    {
        private readonly IProblemAccountBySportDataAccess _dataAccess;

        public ProblemAccountBySportService(IProblemAccountBySportDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IEnumerable<ProblemAccountLosingBySportEntity> GetPALosingBySport(IEnumerable<BaseCustomerInfoBySportModel> custInfos)
        {
            if (custInfos.IsNotNullOrEmpty())
            {
                var custInfosJson = JsonConvert.SerializeObject(custInfos);

                return _dataAccess.GetPALosingBySport(custInfosJson);
            }

            return Enumerable.Empty<ProblemAccountLosingBySportEntity>();
        }
    }
}
