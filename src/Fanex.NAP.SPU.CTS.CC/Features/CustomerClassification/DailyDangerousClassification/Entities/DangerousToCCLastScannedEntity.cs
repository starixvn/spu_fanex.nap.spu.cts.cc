namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.DailyDangerousClassification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DangerousToCCLastScannedEntity
    {
        public DateTime? LastScannedDate { get; set; }

        public long LastCustID { get; set; }
    }
}
