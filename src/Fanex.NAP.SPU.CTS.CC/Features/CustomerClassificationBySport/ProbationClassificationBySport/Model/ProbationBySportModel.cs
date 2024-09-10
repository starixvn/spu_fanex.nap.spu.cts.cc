namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.ProbationClassificationBySport
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProbationBySportModel
    {
        public ProbationBySportModel()
        {
            this.CustProbations = Enumerable.Empty<ProbationScanClassificationBySportEntity>();
        }

        public IEnumerable<ProbationScanClassificationBySportEntity> CustProbations { get; set; }

        public long LastCustID { get; set; }
    }
}
