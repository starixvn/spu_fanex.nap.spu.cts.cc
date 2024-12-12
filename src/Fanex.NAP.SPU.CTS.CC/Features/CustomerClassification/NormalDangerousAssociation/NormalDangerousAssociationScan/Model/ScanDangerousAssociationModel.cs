namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Model
{
    using System.Collections.Generic;

    public class ScanDangerousAssociationModel
    {
        public IEnumerable<long> CustIds { get; set; }

        public long LastScannedID { get; set; }
    }
}
