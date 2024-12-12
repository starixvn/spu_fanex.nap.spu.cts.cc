namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Model
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationCheck.Entities;
    using System.Collections.Generic;

    public class ScanAssociationByDeviceModel
    {
        public long LastCTSAssDevID { get; set; }

        public IEnumerable<ScanAssociationByDeviceEntity> Data { get; set; }
    }
}
