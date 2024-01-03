namespace Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.ClassificationJobMonitor
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class NormalClassificationPoolMonitorGetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "GeneralPoolCount")]
        public long GeneralPoolCount { get; set; }

        [SpParam(SqlParamName = "BySportPoolCount")]
        public long BySportPoolCount { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_Monitoring_CustomerClassificationPool_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}