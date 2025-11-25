namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.MatchMonitorClassificationBySport
{
    using System.Collections.Generic;

    public class ClassifyGBMatchMonitorBySportModel
    {
        public IEnumerable<ClassifyGBMMCustInfosBySportEntity> CustInfos { get; set; }

        public string MMDetailIDList { get; set; }

        public int? MatchID { get; set; }
    }
}
