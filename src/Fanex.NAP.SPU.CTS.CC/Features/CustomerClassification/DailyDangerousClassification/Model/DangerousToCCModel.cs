namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification
{
    using System.Collections.Generic;

    public class DangerousToCCModel
    {       
        public DangerousToCCLastScannedEntity DangerousCCLastScanned { get; set; }

        public IEnumerable<DangerousToCCEntity> DangerousCCs { get; set; }
    }
}
