namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Model
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeTWInitialSmartBySportService.Entities;
    using System;
    using System.Collections.Generic;

    public class TWInitialSmartBySportModel
    {
        public IEnumerable<TWInitialSmartBySportEntity> CustInfos { get; set; }

        public DateTime NextScannedDateTime { get; set; }

        public long NextScannedCustID { get; set; }
    }
}
