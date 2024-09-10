namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ProbationClassification
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProbationModel
    {
        public ProbationModel()
        {
            this.CustProbations = Enumerable.Empty<ProbationEntity>();
        }

        public IEnumerable<ProbationEntity> CustProbations { get; set; }

        public long LastCustID { get; set; }

        public int LastCategoryID { get; set; }
    }
}
