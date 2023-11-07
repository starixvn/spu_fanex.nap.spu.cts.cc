namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.Renovate.NewTWGroupBettingClassification.Model
{
    using System;
    using System.Collections.Generic;

    public class TWGBTaggingCheckChangeModel
    {
        public IEnumerable<long> CustIDs { get; set; }

        public DateTime NextScannedDateTime { get; set; }
    }
}
