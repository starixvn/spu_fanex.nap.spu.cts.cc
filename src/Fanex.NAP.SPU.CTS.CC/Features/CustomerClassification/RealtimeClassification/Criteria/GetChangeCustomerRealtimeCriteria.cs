namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.RealtimeClassification
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetChangeCustomerRealtimeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_GeneralNormalClassification_RealTime_GetChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
