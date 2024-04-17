namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.TWInitialGroupBetting
{
    using System;
    using System.Collections.Generic;

    public class TWInitialGBGetChangesModel
    {
        public IEnumerable<TWInitialGBGetChangesEntity> CustIDs { get; set; }

        public DateTime NextScannedDateTime { get; set; }
    }
}
