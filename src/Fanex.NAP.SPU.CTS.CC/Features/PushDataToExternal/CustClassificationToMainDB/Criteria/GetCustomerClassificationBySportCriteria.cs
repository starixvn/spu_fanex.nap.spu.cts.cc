namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIDs { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_BySport_GetCCAndDangerLevel";

        public override bool IsValid()
            => true;
    }
}
