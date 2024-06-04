namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using System.Collections.Generic;

    public class PADailyScanFromQueueModel
    {
        public IEnumerable<ulong> PADailyScanCustIDs { get; set; }

        public IEnumerable<CustToBuildRemarkEntity> PAPotentialScanCustIDs { get; set; }
    }
}
