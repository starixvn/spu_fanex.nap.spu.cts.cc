namespace Fanex.NAP.SPU.CTS.CC.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetCustomerClassificationBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Custs")]
        public string CustInfos { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_BySport_GetCC";

        public override bool IsValid()
            => true;
    }
}
