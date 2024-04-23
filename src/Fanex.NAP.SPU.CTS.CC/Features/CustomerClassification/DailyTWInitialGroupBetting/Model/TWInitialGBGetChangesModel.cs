namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyTWInitialGroupBetting
{
    using System;
    using System.Collections.Generic;

    public class TWInitialGBGetChangesModel
    {
        public IEnumerable<TWInitialGBGetChangesEntity> CustInfos { get; set; }

        public DateTime NextScannedDateTime { get; set; }

        public long NextScannedCustID { get; set; }
    }
}
