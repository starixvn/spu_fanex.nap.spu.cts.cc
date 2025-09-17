namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyProblemClassification.Entities;
    using System.Collections.Generic;

    public class PADailyScanFromQueueModel
    {
        public IEnumerable<PADailyScanFromQueueEntity> PADailyScanCustIDs { get; set; }
    }
}
