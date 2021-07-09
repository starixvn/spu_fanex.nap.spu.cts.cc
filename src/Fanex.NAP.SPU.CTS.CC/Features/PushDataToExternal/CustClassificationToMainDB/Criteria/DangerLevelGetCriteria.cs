namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class DangerLevelGetCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_CustIDs")]
        public string CustIds { get; set; }

        [SpParam(SqlParamName = "ip_TypeID")]
        public int TypeID { get; set; }

        public override string GetSettingKey()
            => "CTS_DC_CustClassification_GetDangerLevel";

        public override bool IsValid()
            => true;
    }
}