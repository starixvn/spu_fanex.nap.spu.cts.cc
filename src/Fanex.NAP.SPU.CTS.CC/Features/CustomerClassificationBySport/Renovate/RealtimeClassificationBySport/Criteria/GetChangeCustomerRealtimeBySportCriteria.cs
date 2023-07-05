namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassificationBySport.Renovate.RealtimeClassificationBySport
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetChangeCustomerRealtimeBySportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ScannedMaxId")]
        public long ScannedMaxId { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_BySportNormalClassification_RealTime_GetChanges";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
