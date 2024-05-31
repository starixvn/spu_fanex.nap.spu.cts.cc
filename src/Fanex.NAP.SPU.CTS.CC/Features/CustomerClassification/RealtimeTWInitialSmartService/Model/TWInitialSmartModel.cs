namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWEarlyBlockService
{
    using System;
    using System.Collections.Generic;

    public class TWInitialSmartModel
    {
        public IEnumerable<TWInitialSmartEntity> CustInfos { get; set; }

        public DateTime NextScannedDateTime { get; set; }

        public long NextScannedCustID { get; set; }
    }
}
