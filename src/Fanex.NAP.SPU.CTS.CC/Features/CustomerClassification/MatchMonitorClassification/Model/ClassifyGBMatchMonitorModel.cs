namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.MatchMonitorClassification
{
    using System.Collections.Generic;

    public class ClassifyGBMatchMonitorModel
    {
        public IEnumerable<ClassifyGBMMCustInfosEntity> CustInfos { get; set; }

        public IEnumerable<int> MMDetailIDs { get; set; }

        public int? MatchID { get; set; }
    }
}
