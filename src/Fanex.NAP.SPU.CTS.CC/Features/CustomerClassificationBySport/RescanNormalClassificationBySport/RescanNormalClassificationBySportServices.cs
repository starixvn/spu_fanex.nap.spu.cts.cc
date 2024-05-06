namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.RescanNormalClassificationBySport
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.CTS.Shares.Constants;

    public interface IRescanNormalClassificationBySportServices
    {
        void PreprocessRescanNormalBySportClassification(IEnumerable<long> custIds, BySport_FunctionId functionId);
    }

    public class RescanNormalClassificationBySportServices : IRescanNormalClassificationBySportServices
    {
        private readonly IRescanNormalClassificationBySportDataAccess dataAccess;


        public RescanNormalClassificationBySportServices(IRescanNormalClassificationBySportDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public void PreprocessRescanNormalBySportClassification(IEnumerable<long> custIds, BySport_FunctionId functionId)
        {
            if (custIds != null && custIds.Any())
            {
                var custToString = string.Join(",", custIds.Distinct());

                this.dataAccess.PreprocesRescanNormalClassificationBySport(custToString, functionId);                
            }
        }
    }
}
