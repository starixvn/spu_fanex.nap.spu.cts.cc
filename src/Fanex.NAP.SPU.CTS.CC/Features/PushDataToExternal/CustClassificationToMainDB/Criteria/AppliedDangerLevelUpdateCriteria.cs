namespace Fanex.NAP.SPU.CTS.Features.PushDataToExternal.CustClassificationToMainDB
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class AppliedDangerLevelUpdateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_ProcessedCustList")]
        public string CustJson { get; set; }

        [SpParam(SqlParamName = "ip_TypeID")]
        public int TypeID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_UpdateDangerLevel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}