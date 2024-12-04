namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalDangerousAssociation.NormalDangerousAssociationScan.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class CompleteDangerousAssociationCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LastScannedID")]
        public long LastScannedID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_DgrAssociation_DevicePool_Clear";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
